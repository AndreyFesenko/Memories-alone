//C:\Users\user\Source\Repos\Memories-alone\src\AuditLoggingService\AuditLoggingService.API\Program.cs
using System.Text.Json.Serialization;
using MassTransit;
using AuditLoggingService.Application;
using AuditLoggingService.Infrastructure;
using AuditLoggingService.Application.Consumers;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// JSON enum as strings
builder.Services.AddControllers()
    .AddJsonOptions(o => {
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
        var host = rabbitCfg["Host"] ?? throw new ArgumentNullException("RabbitMQ:Host is not set");
        var vhost = rabbitCfg["VirtualHost"] ?? "/";
        var user = rabbitCfg["User"] ?? "guest";
        var pass = rabbitCfg["Password"] ?? "guest";
        var queue = rabbitCfg["Queue"] ?? "audit-log-queue";
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
                s.RoutingKey = "audit.*"; // слушаем события для аудита
            });

            e.ConfigureConsumer<AuditLogMessageConsumer>(context);
        });
    });
});

// HealthChecks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("Default")!,
        name: "postgres",
        tags: new[] { "ready" }
    );

// Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .WriteTo.Console() // <-- вывод в консоль
      .Enrich.FromLogContext();
});

var app = builder.Build();

// Middleware & endpoints
app.UseHttpsRedirection();
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
