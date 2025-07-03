using MassTransit;
using Microsoft.AspNetCore.RateLimiting;
using NotificationService.Application;
using NotificationService.Application.Hubs;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.Services;
using static MassTransit.Monitoring.Performance.BuiltInCounters;
using NotificationService.Application.Consumers;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// DI дл€ слоев приложени€
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Swagger, SignalR, CORS
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAll", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true));
});

// Rate Limiting Ч только один вызов AddRateLimiter
builder.Services.AddRateLimiter(options =>
{
    // √лобальный лимит по IP или User.Identity.Name
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

    // »менованна€ политика дл€ атрибутов
    options.AddFixedWindowLimiter("notifications", opts =>
    {
        opts.PermitLimit = 10;
        opts.Window = TimeSpan.FromMinutes(1);
        opts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opts.QueueLimit = 3;
    });
});

// MassTransit + RabbitMQ + Consumer
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotificationMessageConsumer>(); // <-- ”кажи свой реальный consumer!
    // x.AddConsumer<ƒругиеConsumers>(); // если есть другие

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("notifications", e =>
        {
            e.ConfigureConsumer<NotificationMessageConsumer>(context);
        });
    });
});

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

// Middleware, Endpoints, CORS
app.UseMiddleware<ErrorHandlingMiddleware>(); // об€зательно реализуй!
app.UseSwagger();
app.UseSwaggerUI();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");
app.UseCors("AllowAll");

app.Run();
