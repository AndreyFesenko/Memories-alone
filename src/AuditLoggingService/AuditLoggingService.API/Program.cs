using AuditLoggingService.Application;
using AuditLoggingService.Application.Consumers;
using AuditLoggingService.Infrastructure;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using System.Net.Sockets;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// JSON enum as strings
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration.GetConnectionString("Default")!);

// OpenAPI + Scalar
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

// MassTransit + RabbitMQ
var rabbitCfg = builder.Configuration.GetSection("RabbitMq");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AuditLogMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = rabbitCfg["Host"] ?? throw new ArgumentNullException("RabbitMq:Host is not set");
        var vhost = rabbitCfg["VirtualHost"] ?? "/";
        var user = rabbitCfg["User"] ?? "guest";
        var pass = rabbitCfg["Password"] ?? "guest";
        var queue = rabbitCfg["Queue"] ?? "audit-log-queue";
        var exchange = rabbitCfg["Exchange"] ?? "notifications";

        var dlx = rabbitCfg["DeadLetterExchange"] ?? "audit.dlx";
        var dlq = rabbitCfg["DeadLetterQueue"] ?? "audit.dlq";
        var dlrk = rabbitCfg["DeadLetterRoutingKey"] ?? "audit.dead";

        cfg.Host(host, vhost, h =>
        {
            h.Username(user);
            h.Password(pass);
        });

        cfg.ReceiveEndpoint(queue, e =>
        {
            e.PrefetchCount = 32;
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            e.SetQueueArgument("x-dead-letter-exchange", dlx);
            e.SetQueueArgument("x-dead-letter-routing-key", dlrk);

            e.ConfigureConsumeTopology = false;
            e.Bind(exchange, s =>
            {
                s.ExchangeType = "topic";
                s.RoutingKey = "audit.*";
            });

            e.ConfigureConsumer<AuditLogMessageConsumer>(context);
        });

        // Объявим DLQ без консьюмера — для корректной маршрутизации dead-letter-сообщений
        cfg.ReceiveEndpoint(dlq, e =>
        {
            e.PrefetchCount = 1;
            e.ConfigureConsumeTopology = false;
        });
    });
});

// HealthChecks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("Default")!,
        name: "postgres",
        tags: new[] { "ready" }
    )
    .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: new[] { "ready" });

// Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .WriteTo.Console()
      .Enrich.FromLogContext();
});

var app = builder.Build();

// Middleware & endpoints
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health endpoints
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Scalar + OpenAPI only in dev
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();


// ─────────────────────────────────────────────────────────────────────────────
// Классы и пространства имён должны идти ПОСЛЕ top-level кода (исправляет CS8803)
// ─────────────────────────────────────────────────────────────────────────────

public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IConfiguration _cfg;
    public RabbitMqHealthCheck(IConfiguration cfg) => _cfg = cfg;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var rcfg = _cfg.GetSection("RabbitMq");
            var host = rcfg["Host"] ?? "localhost";
            var portS = rcfg["Port"] ?? "5672";
            if (!int.TryParse(portS, out var port)) port = 5672;

            using var client = new TcpClient();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(3)); // быстрый таймаут

            var connectTask = client.ConnectAsync(host, port);
            var first = await Task.WhenAny(connectTask, Task.Delay(Timeout.Infinite, cts.Token));
            if (first != connectTask || !client.Connected)
                return HealthCheckResult.Unhealthy($"RabbitMQ TCP not reachable at {host}:{port}");

            return HealthCheckResult.Healthy("RabbitMQ OK");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ fail", ex);
        }
    }
}
