using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;

namespace AuditLoggingService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Подключение MediatR и других сервисов Application-слоя
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        // TODO: Добавь другие application-сервисы, например валидации

        return services;
    }
}
