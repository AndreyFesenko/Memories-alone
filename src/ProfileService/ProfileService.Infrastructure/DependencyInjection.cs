using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProfileService.Application.Interfaces;
using ProfileService.Infrastructure.Repositories;

namespace ProfileService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProfilesDbContext>(opts =>
        {
            var cs = configuration.GetConnectionString("Default")
                     ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured");
            opts.UseNpgsql(cs, npg => npg.MigrationsHistoryTable("__EFMigrationsHistory", "profile"));
        });

        services.AddScoped<IProfileRepository, ProfileRepository>();
        return services;
    }
}
