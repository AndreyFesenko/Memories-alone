using AuditLoggingService.Application;
using AuditLoggingService.Infrastructure;
using AuditLoggingService.Application.Consumers;
using Scalar.AspNetCore;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 👇 Логирование через Serilog (по желанию)
builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

// 👇 Подключаем контроллеры и Scalar/OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

// 👇 DI: Application + Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration.GetConnectionString("Default")!);

// 👇 MassTransit + RabbitMQ
var rabbitSection = builder.Configuration.GetSection("RabbitMq");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AuditLogMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitSection["Host"], rabbitSection["VirtualHost"] ?? "/", h =>
        {
            h.Username(rabbitSection["User"]);
            h.Password(rabbitSection["Password"]);
        });

        cfg.ReceiveEndpoint(rabbitSection["Queue"] ?? "audit-log-queue", e =>
        {
            e.ConfigureConsumer<AuditLogMessageConsumer>(context);
        });
    });
});

var app = builder.Build();

// 👇 DEV-only endpoints
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
