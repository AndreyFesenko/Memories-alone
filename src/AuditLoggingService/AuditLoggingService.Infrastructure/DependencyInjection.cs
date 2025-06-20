using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AuditLoggingService.Application.Interfaces;
using AuditLoggingService.Infrastructure.Persistence;
using AuditLoggingService.Infrastructure.Repositories;

namespace AuditLoggingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AuditLoggingDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        // Добавь другие инфраструктурные сервисы если нужно

        return services;
    }
}
