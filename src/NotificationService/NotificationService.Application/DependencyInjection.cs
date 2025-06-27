using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;


namespace NotificationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyReference>();

        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
