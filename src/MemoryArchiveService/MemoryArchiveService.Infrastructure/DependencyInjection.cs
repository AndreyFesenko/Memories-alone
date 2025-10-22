using Amazon.S3;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Infrastructure.Persistence;
using MemoryArchiveService.Infrastructure.Repositories;
using MemoryArchiveService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryArchiveService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // Db
        services.AddDbContext<MemoryArchiveDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        // Repos
        services.AddScoped<IMemoryRepository, MemoryRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        // Read-store (для CQRS чтения в DTO)
        services.AddScoped<IMediaReadStore, MediaReadStore>();

        // RabbitMQ
        services.Configure<RabbitMqOptions>(config.GetSection("RabbitMq"));
        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        // Supabase S3
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>().GetSection("Supabase:S3");
            var serviceUrl = cfg["Endpoint"];
            var access = cfg["AccessKey"];
            var secret = cfg["SecretKey"];

            var s3cfg = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
                AuthenticationRegion = "ap-southeast-1"
            };

            return new AmazonS3Client(access, secret, s3cfg);
        });

        services.AddScoped<IStorageService, SupabaseStorageService>();
        services.AddSingleton<IPublicUrlResolver, SupabasePublicUrlResolver>();

        return services;
    }
}
