﻿// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/DependencyInjection.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Minio;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Infrastructure.Persistence;
using MemoryArchiveService.Infrastructure.Repositories;
using MemoryArchiveService.Infrastructure.Services;
using Amazon;
using Amazon.S3;

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

        // MinIO
        services.AddSingleton<IMinioClient>(sp =>
        {
            var minioConfig = config.GetSection("Minio");
            var endpoint = minioConfig.GetValue<string>("Endpoint") ?? "localhost:9000";
            var accessKey = minioConfig.GetValue<string>("AccessKey") ?? "minioadmin";
            var secretKey = minioConfig.GetValue<string>("SecretKey") ?? "minioadmin";
            var useSSL = minioConfig.GetValue<bool?>("UseSSL") ?? false;
            return new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(useSSL)
                .Build();
        });
        services.AddScoped<IFileStorageService, MinioStorageService>();
        services.AddScoped<IMediaStorageService, MediaStorageService>();

        // RabbitMQ Event Bus (через опции)
        services.Configure<RabbitMqOptions>(config.GetSection("RabbitMq"));
        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new AmazonS3Client(
                config["Supabase:S3:AccessKey"],
                config["Supabase:S3:SecretKey"],
                new AmazonS3Config
                {
                    ServiceURL = config["Supabase:S3:Endpoint"],
                    ForcePathStyle = true
                });
        });

        services.AddScoped<IStorageService, SupabaseStorageService>();

        return services;
    }
}
