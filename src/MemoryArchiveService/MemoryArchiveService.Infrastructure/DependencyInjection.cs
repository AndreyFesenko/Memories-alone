// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/DependencyInjection.cs
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
        // PostgreSQL
        services.AddDbContext<MemoryArchiveDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        // Repositories
        services.AddScoped<IMemoryRepository, MemoryRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        // RabbitMQ Event Bus
        services.Configure<RabbitMqOptions>(config.GetSection("RabbitMq"));
        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        // Supabase S3
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            return new AmazonS3Client(
                cfg["Supabase:S3:AccessKey"],
                cfg["Supabase:S3:SecretKey"],
                new AmazonS3Config
                {
                    ServiceURL = cfg["Supabase:S3:Endpoint"], // https://...supabase.co/storage/v1/s3
                    ForcePathStyle = true,                     // обязательно для S3-совместимых API
                    //SignatureVersion = "4"                     // явное V4-подписание (безопасно)
                    // AuthenticationRegion можно не указывать с ServiceURL
                });
        });

        // Хранилище на базе IAmazonS3
        services.AddScoped<IStorageService, SupabaseStorageService>();

        return services;
    }
}
