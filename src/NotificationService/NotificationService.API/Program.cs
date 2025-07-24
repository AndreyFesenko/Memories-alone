using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using MassTransit;
using Microsoft.AspNetCore.RateLimiting;
using NotificationService.Application;
using NotificationService.Application.Consumers;
using NotificationService.Application.Hubs;
using NotificationService.Infrastructure;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// JSON enum as strings
builder.Services.AddControllers()
    .AddJsonOptions(o => {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// --- SignalR + Redis Backplane ---
builder.Services.AddSignalR()
    .AddStackExchangeRedis(builder.Configuration.GetConnectionString("Redis")!);


builder.Services.AddCors(opts => {
    opts.AddPolicy("AllowAll", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true));
});

// RateLimiter
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var user = context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anon";
        return RateLimitPartition.GetFixedWindowLimiter(user, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromSeconds(10),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 2
        });
    });
    options.AddFixedWindowLimiter("notifications", opts =>
    {
        opts.PermitLimit = 10;
        opts.Window = TimeSpan.FromMinutes(1);
        opts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opts.QueueLimit = 3;
    });
});

// OpenAPI + Scalar
builder.Services.AddOpenApi();

// MassTransit + RabbitMQ
var rabbitCfg = builder.Configuration.GetSection("RabbitMq");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotificationMessageConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            rabbitCfg["Host"],
            rabbitCfg["VirtualHost"] ?? "/",
            h =>
            {
                h.Username(rabbitCfg["User"]);
                h.Password(rabbitCfg["Password"]);
            });

        cfg.ReceiveEndpoint(rabbitCfg["Queue"], e =>
        {
            e.ConfigureConsumer<NotificationMessageConsumer>(context);
        });
    });
});

// RabbitMQ Connection Factory
builder.Services.AddSingleton<ConnectionFactory>(sp => new ConnectionFactory()
{
    Uri = new Uri($"amqp://{rabbitCfg["User"]}:{rabbitCfg["Password"]}@{rabbitCfg["Host"]}/{rabbitCfg["VirtualHost"] ?? "/"}")
});

// RabbitMQ Connection Service
builder.Services.AddSingleton<RabbitMqConnectionService>();
builder.Services.AddSingleton<IConnection>(sp => sp.GetRequiredService<RabbitMqConnectionService>().Connection);

// Health Checks — используем теги!
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("Default")!,
        name: "postgres",
        tags: new[] { "ready" }
    )
    .AddRabbitMQ(
        factory: sp => sp.GetRequiredService<IConnection>(),
        name: "rabbitmq",
        tags: new[] { "ready" }
    );

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

// Инициализация RabbitMQ
var rabbitMqService = app.Services.GetRequiredService<RabbitMqConnectionService>();
await rabbitMqService.InitializeAsync();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");
app.UseRateLimiter();
app.UseCors("AllowAll");
app.UseAuthorization();

// ---- HEALTH endpoints ----
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // Просто факт, что сервис поднят
});
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Scalar и OpenAPI только в DEV/DEBUG
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();

/// <summary>
/// Service to handle persistent RabbitMQ connection.
/// </summary>
public class RabbitMqConnectionService : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnectionService(ConnectionFactory factory)
    {
        _factory = factory;
    }

    public IConnection Connection => _connection ?? throw new InvalidOperationException("RabbitMQ connection not initialized");

    public async Task InitializeAsync()
    {
        try
        {
            _connection = await _factory.CreateConnectionAsync();
            Log.Information("RabbitMQ connection established");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to establish RabbitMQ connection");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
    }
}
