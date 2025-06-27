using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;

namespace MemoryArchiveService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Регистрация MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        // Здесь можно регистрировать мапперы, валидаторы и т.п.

        return services;
    }
}
