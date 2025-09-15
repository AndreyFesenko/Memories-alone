// src/IdentityService/IdentityService.Application/DependencyInjection.cs
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IdentityService.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрация сервисов слоя Application:
    /// - MediatR (Handlers/Requests из сборки Application)
    /// - FluentValidation (+ авто-валидация для MVC)
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MediatR: регистрируем все обработчики из этой сборки
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        // FluentValidation: валидаторы из этой сборки + авто-валидация MVC
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddFluentValidationAutoValidation();

        return services;
    }
}
