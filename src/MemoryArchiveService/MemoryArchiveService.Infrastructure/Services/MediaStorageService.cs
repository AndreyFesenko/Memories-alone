using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using MemoryArchiveService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MemoryArchiveService.Infrastructure.Services;

/// <summary>
/// Реализация IMediaStorageService через Amazon S3 SDK (Supabase S3 совместимый API).
/// </summary>
public class MediaStorageService : IMediaStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;
    private readonly string? _publicBaseUrl; // опционально: если бакет публичный

    public MediaStorageService(IAmazonS3 s3, IConfiguration cfg)
    {
        _s3 = s3;
        _bucket = cfg["Supabase:S3:Bucket"]
            ?? throw new InvalidOperationException("Config Supabase:S3:Bucket is missing");
        // Если настроен публичный доступ к бакету через CDN/публичный URL — укажи это в конфиге:
        // "Supabase:S3:PublicBaseUrl": "https://<project>.supabase.co/storage/v1/object/public"
        _publicBaseUrl = cfg["Supabase:S3:PublicBaseUrl"];
    }

    public async Task<string> UploadAsync(string fileName, Stream fileStream, string contentType, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            contentType = MediaTypeNames.Application.Octet;

        // Загрузка объекта
        var putReq = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType
        };
        // (опционально) сделать объект публичным, если бакет приватный, но нужен паблик:
        // putReq.Headers["x-amz-acl"] = "public-read"; // работает не во всех S3-совместимых API

        var putResp = await _s3.PutObjectAsync(putReq, ct);

        // Вернём URL:
        // 1) Если задан PublicBaseUrl — конструируем публичный URL
        if (!string.IsNullOrWhiteSpace(_publicBaseUrl))
            return $"{_publicBaseUrl.TrimEnd('/')}/{_bucket}/{Uri.EscapeDataString(fileName)}";

        // 2) Иначе — пресайненная ссылка на 1 час
        var urlReq = new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = fileName,
            Expires = DateTime.UtcNow.AddHours(1),
            Verb = HttpVerb.GET
        };
        return _s3.GetPreSignedURL(urlReq);
    }

    public async Task<Stream> DownloadAsync(string fileName, CancellationToken ct = default)
    {
        var resp = await _s3.GetObjectAsync(new GetObjectRequest
        {
            BucketName = _bucket,
            Key = fileName
        }, ct);

        var ms = new MemoryStream();
        await resp.ResponseStream.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }

    public Task DeleteAsync(string fileName, CancellationToken ct = default)
    {
        return _s3.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _bucket,
            Key = fileName
        }, ct);
    }
}
