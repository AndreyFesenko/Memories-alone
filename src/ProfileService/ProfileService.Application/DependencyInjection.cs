﻿// Application/DependencyInjection.cs
using Microsoft.Extensions.DependencyInjection;


namespace ProfileService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            // Здесь добавлять другие сервисы, валидации, маппинги
            return services;
        }
    }
}
