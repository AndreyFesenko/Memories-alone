using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation; // вот это using добавь

namespace ProfileService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Подключаем MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Подключаем все валидаторы из этого сборника
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
