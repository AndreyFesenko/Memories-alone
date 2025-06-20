// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Services/MinioStorageService.cs

using Minio;
using Minio.DataModel.Args;
using MemoryArchiveService.Application.Interfaces;
using System.IO;

namespace MemoryArchiveService.Infrastructure.Services;

public class MinioStorageService : IFileStorageService
{
    private readonly IMinioClient _client;    // <= смотри, тип для DI — IMinioClient (новая рекомендация MinIO)
    private readonly string _bucketName = "memory-media";

    public MinioStorageService(IMinioClient client)
    {
        _client = client;
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct)
    {
        // Проверка существования bucket и создание если нужно
        bool found = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName), ct);

        if (!found)
            await _client.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucketName), ct);

        // Загрузка файла
        await _client.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType),
            ct);

        // Вернём только путь (пример), зависит от конфигурации публичного доступа
        return $"/{_bucketName}/{fileName}";
    }

    // Пример удаления файла
    public async Task DeleteAsync(string fileName, CancellationToken ct)
    {
        await _client.RemoveObjectAsync(
            new RemoveObjectArgs().WithBucket(_bucketName).WithObject(fileName),
            ct);
    }
}
