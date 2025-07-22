using MassTransit;
using Microsoft.AspNetCore.RateLimiting;
using NotificationService.Application;
using NotificationService.Application.Hubs;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.Services;
using NotificationService.Application.Consumers;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// JSON enum as strings
builder.Services.AddControllers()
    .AddJsonOptions(o => {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddCors(opts => {
    opts.AddPolicy("AllowAll", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true));
});

// RateLimiter (один раз!)
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

// --- OpenAPI + Scalar ---
builder.Services.AddOpenApi(); // генерирует swagger.json и включает Scalar UI

// MassTransit + RabbitMQ + Consumer
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

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// »спользуем только Scalar (Swagger UI и app.UseSwaggerUI() убраны)
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");
app.UseRateLimiter();
app.UseCors("AllowAll");
app.UseAuthorization();

// Scalar и OpenAPI только в DEV/DEBUG (можно убрать условие дл€ PROD)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                // генерируем openapi/v1.json
    app.MapScalarApiReference();     // UI: /scalar/v1
    // ћожно: app.MapScalarApiReference("/docs"); // если нужен кастомный путь
}

app.Run();
