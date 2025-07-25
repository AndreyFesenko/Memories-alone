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
        // 👇 Регистрируем DbContext с PostgreSQL-провайдером
        services.AddDbContext<AuditLoggingDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AuditLoggingDbContext).Assembly.FullName);
            }));

        // 👇 Добавляем репозиторий
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        return services;
    }
}
