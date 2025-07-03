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
        services.AddSingleton<ITemplateRenderer, TemplateRenderer>();
        services.AddScoped<IEmailSender, EmailNotificationSender>();



        //для динамического рендеринга шаблонов уведомлений.
        services.AddSingleton<ITemplateRenderer, TemplateRenderer>();

        return services;
    }
}
