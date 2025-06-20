// Infrastructure/DependencyInjection.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ProfileService.Infrastructure.Repositories;
using ProfileService.Application.Interfaces;


namespace ProfileService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProfilesDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Default")));
            services.AddScoped<IProfileRepository, ProfileRepository>();
            return services;
        }
    }
}
