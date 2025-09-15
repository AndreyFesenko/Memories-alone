// src/IdentityService/IdentityService.Infrastructure/DependencyInjection.cs
using IdentityService.Application.Interfaces;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрация сервисов уровня Infrastructure:
    /// - DbContext (Npgsql)
    /// - Репозитории (User/Role)
    /// - Сервисы (Auth/JWT/Refresh/Audit/Profile stub)
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        var connStr = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is missing");
        services.AddDbContext<MemoriesDbContext>(opts => opts.UseNpgsql(connStr));

        // Репозитории
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // Сервисы (то, что было в Program.cs)
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IProfileServiceClient, ProfileServiceStub>();

        return services;
    }
}
