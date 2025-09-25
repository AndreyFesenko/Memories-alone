// src\AccessControlService\AccessControlService.Infrastructure\DependencyInjection.cs
using AccessControlService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControlService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccessInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AccessDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<AccessRuleRepository>();
        services.AddScoped<AuditLogRepository>();

        return services;
    }
}
