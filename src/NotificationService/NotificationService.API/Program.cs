// src/NotificationService/NotificationService.API/Program.cs
using MassTransit;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NotificationService.Application;
using NotificationService.Application.Consumers;
using NotificationService.Application.Hubs;
using NotificationService.Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// === Конфиг: ТОЛЬКО JSON (без ENV/CLI override) ===
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

// === JSON enums как строки ===
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// === App services ===
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// === Redis backplane (SignalR) через connection string ===
var redisCfg = builder.Configuration.GetSection("Redis");
string redisConnString =
    $"{redisCfg["Host"]}:{redisCfg["Port"]},abortConnect=False" +
    (redisCfg.GetValue<bool>("Ssl") ? ",ssl=True" : ",ssl=False") +
    (!string.IsNullOrWhiteSpace(redisCfg["Password"]) ? $",password={redisCfg["Password"]}" : "");

builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConnString);

// === CORS ===
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAll", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true));
});

// === Rate Limiting (глобальный + именованная политика 'notifications') ===
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var key = context.User.Identity?.Name
                  ?? context.Connection.RemoteIpAddress?.ToString()
                  ?? "anon";
        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromSeconds(10),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 2
        });
    });

    options.AddPolicy("notifications", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User?.Identity?.Name
                          ?? httpContext.Connection.RemoteIpAddress?.ToString()
                          ?? "anon",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

// === OpenAPI + Scalar ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// === MassTransit + RabbitMQ ===
var rabbitCfg = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotificationMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = rabbitCfg["Host"] ?? throw new ArgumentNullException("RabbitMq:Host is not set");
        var vhost = rabbitCfg["VirtualHost"] ?? "/";
        var user = rabbitCfg["User"] ?? "guest";
        var pass = rabbitCfg["Password"] ?? "guest";
        var queue = rabbitCfg["Queue"] ?? "notifications.queue";
        var exch = rabbitCfg["Exchange"] ?? "notifications";

        cfg.Host(host, vhost, h =>
        {
            h.Username(user);
            h.Password(pass);
        });

        cfg.ReceiveEndpoint(queue, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind(exch, s =>
            {
                s.ExchangeType = "topic";
                s.RoutingKey = "notification.*";
            });

            e.ConfigureConsumer<NotificationMessageConsumer>(context);
        });
    });
});

// === HealthChecks: Postgres / RabbitMQ / Redis ===
string Pg() => builder.Configuration.GetConnectionString("Default")!;

string Amqp()
{
    var host = rabbitCfg["Host"] ?? "localhost";
    var vhost = Uri.EscapeDataString(rabbitCfg["VirtualHost"] ?? "/"); // "/" -> "%2F"
    var user = rabbitCfg["User"] ?? "guest";
    var pass = rabbitCfg["Password"] ?? "guest";
    return $"amqp://{user}:{pass}@{host}/{vhost}";
}

builder.Services.AddHealthChecks()
    .AddNpgSql(Pg(), name: "postgres", tags: new[] { "ready" })
    .AddRabbitMQ(
        factory: sp =>
        {
            var f = new RabbitMQ.Client.ConnectionFactory { Uri = new Uri(Amqp()) };
            // HealthCheck ожидает sync-фабрику — аккуратно оборачиваем async:
            return f.CreateConnectionAsync().GetAwaiter().GetResult();
        },
        name: "rabbitmq",
        tags: new[] { "ready" }
    )
    .AddRedis(redisConnString, name: "redis", tags: new[] { "ready" });

// === Serilog ===
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration)
      .WriteTo.Console()
      .Enrich.FromLogContext());

var app = builder.Build();

// === Middleware порядок ===
// Без UseHttpsRedirection в dev (чтобы не было 307 через Gateway)
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Health endpoints (добавили /health для Render)
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

// Swagger & Scalar (dev only)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();
