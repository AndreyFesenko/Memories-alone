// src/NotificationService/NotificationService.Application/DependencyInjection.cs
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;

namespace NotificationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        // Validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // AutoMapper
        services.AddAutoMapper(cfg => { /* опционально: cfg.AllowNullCollections = true; */ },
    typeof(DependencyInjection).Assembly);

        services.AddValidatorsFromAssemblyContaining<CreateNotificationCommandValidator>();

        return services;
    }
}
