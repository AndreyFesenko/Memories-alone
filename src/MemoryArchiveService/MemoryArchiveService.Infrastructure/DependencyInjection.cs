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
            var cfg = sp.GetRequiredService<IConfiguration>().GetSection("Supabase:S3");

            var serviceUrl = cfg["Endpoint"];         // ✅ правильно
            var access = cfg["AccessKey"];        // ✅ S3-ключи из Storage → S3 keys
            var secret = cfg["SecretKey"];

            var s3cfg = new AmazonS3Config
            {
                ServiceURL = serviceUrl,              // например: https://<project>.supabase.co/storage/v1/s3
                ForcePathStyle = true,                // для совместимых S3 API
                AuthenticationRegion = "ap-southeast-1"    // обязательно для Supabase S3 SDK
            };

            return new AmazonS3Client(access, secret, s3cfg);
        });


        // Хранилище на базе IAmazonS3
        services.AddScoped<IStorageService, SupabaseStorageService>();

        services.AddSingleton<IPublicUrlResolver, SupabasePublicUrlResolver>();

        return services;
    }
}
