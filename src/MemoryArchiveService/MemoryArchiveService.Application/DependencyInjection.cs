using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MemoryArchiveService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Регистрируем все IRequestHandler/INotificationHandler из сборки Application
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}
