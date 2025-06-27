using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // DbContext
        services.AddDbContext<NotificationDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        // Repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();

        // Email (примитивная реализация, можно расширять)
        services.AddScoped<IEmailSender, EmailNotificationSender>();

        // MassTransit (RabbitMQ)
        services.AddMassTransit(x =>
        {
            x.AddConsumer<NotificationConsumer>();
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(config.GetSection("RabbitMq")["Host"] ?? "localhost", "/", h =>
                {
                    h.Username(config.GetSection("RabbitMq")["User"] ?? "guest");
                    h.Password(config.GetSection("RabbitMq")["Password"] ?? "guest");
                });
            });
        });

        // Rate Limiting (пример, если .NET 7+)
        services.AddRateLimiter(_ =>
        {
            _.AddPolicy("notifications", context => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User?.Identity?.Name ?? "anonymous",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromSeconds(5),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 2,
                }));
        });

        return services;
    }
}
