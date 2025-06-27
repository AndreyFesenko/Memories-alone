using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces;
using NotificationService.Infrastructure.Consumers;
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

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();

        // MassTransit/RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumer<NotificationConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                var section = config.GetSection("RabbitMq");
                cfg.Host(section["Host"] ?? "localhost", h =>
                {
                    h.Username(section["User"] ?? "guest");
                    h.Password(section["Password"] ?? "guest");
                });

                cfg.ReceiveEndpoint("notifications.queue", e =>
                {
                    e.ConfigureConsumer<NotificationConsumer>(context);
                    e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
                });
            });
        });

        services.AddScoped<INotificationQueuePublisher, MassTransitNotificationPublisher>();
        services.AddScoped<ITemplateRenderer, HandlebarsTemplateRenderer>();

        return services;
    }
}
