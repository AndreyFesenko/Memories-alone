//C:\Users\user\Source\Repos\Memories-alone\src\NotificationService\NotificationService.API\Program.cs
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
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// JSON enum as strings
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// App services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Redis config
var redisCfg = builder.Configuration.GetSection("Redis");
var redisOptions = new ConfigurationOptions
{
    EndPoints = { $"{redisCfg["Host"]}:{redisCfg["Port"]}" },
    Password = redisCfg["Password"],
    Ssl = redisCfg.GetValue<bool>("Ssl"),
    AbortOnConnectFail = false
};

// SignalR + Redis backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisOptions.ToString());

// Redis connection singleton (для HealthCheck)
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisOptions));

// CORS
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAll", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true));
});

// Rate Limiting
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
        var host = rabbitCfg["Host"] ?? throw new ArgumentNullException("RabbitMQ:Host is not set");
        var vhost = rabbitCfg["VirtualHost"] ?? "/";
        var user = rabbitCfg["User"] ?? "guest";
        var pass = rabbitCfg["Password"] ?? "guest";
        var queue = rabbitCfg["Queue"] ?? "notifications.queue";
        var exchange = rabbitCfg["Exchange"] ?? "notifications";

        cfg.Host(host, vhost, h =>
        {
            h.Username(user);
            h.Password(pass);
        });

        cfg.ReceiveEndpoint(queue, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind(exchange, s =>
            {
                s.ExchangeType = "topic";
                s.RoutingKey = "notification.*";
            });

            e.ConfigureConsumer<NotificationMessageConsumer>(context);
        });
    });
});

// RabbitMQ connection
builder.Services.AddSingleton<ConnectionFactory>(sp => new ConnectionFactory
{
    Uri = new Uri($"amqp://{rabbitCfg["User"]}:{rabbitCfg["Password"]}@{rabbitCfg["Host"]}/{rabbitCfg["VirtualHost"] ?? "/"}")
});

builder.Services.AddSingleton<RabbitMqConnectionService>();
builder.Services.AddSingleton<IConnection>(sp => sp.GetRequiredService<RabbitMqConnectionService>().Connection);

// HealthChecks
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
    )
    .AddRedis(
        redisConnectionString: redisOptions.ToString(),
        name: "redis",
        tags: new[] { "ready" }
    );

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// Build App
var app = builder.Build();

// Init RabbitMQ
var rabbitMqService = app.Services.GetRequiredService<RabbitMqConnectionService>();
await rabbitMqService.InitializeAsync();

// Middleware & endpoints
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");
app.UseRateLimiter();
app.UseCors("AllowAll");
app.UseAuthorization();

// Health endpoints
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Swagger & Scalar (dev only)
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

