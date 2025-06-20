using MemoryArchiveService.Application.Interfaces;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace MemoryArchiveService.Infrastructure.Services;

public class MediaStorageService : IMediaStorageService
{
    private readonly IMinioClient _minio;
    private readonly string _bucket = "memories-media"; // имя бакета

    public MediaStorageService(IMinioClient minio)
    {
        _minio = minio;
    }

    public async Task<string> UploadAsync(string fileName, Stream fileStream, string contentType, CancellationToken ct = default)
    {
        // Создать бакет, если не существует
        bool found = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket), ct);
        if (!found)
            await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket), ct);

        // Загрузить файл
        await _minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithContentType(contentType)
            .WithObjectSize(fileStream.Length), ct);

        // Вернуть публичный URL (настроить доступность бакета в MinIO!)
        return $"http://localhost:9000/{_bucket}/{fileName}";
    }

    public async Task<Stream> DownloadAsync(string fileName, CancellationToken ct = default)
    {
        var ms = new MemoryStream();
        await _minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithCallbackStream(s => s.CopyTo(ms)), ct);
        ms.Position = 0;
        return ms;
    }

    public async Task DeleteAsync(string fileName, CancellationToken ct = default)
    {
        await _minio.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName), ct);
    }
}
