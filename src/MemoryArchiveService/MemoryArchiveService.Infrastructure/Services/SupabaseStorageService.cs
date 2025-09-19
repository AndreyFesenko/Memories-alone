// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Services/SupabaseStorageService.cs
using Amazon.S3;
using Amazon.S3.Model;
using MemoryArchiveService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MemoryArchiveService.Infrastructure.Services;

public class SupabaseStorageService : IStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;
    private readonly string _baseUrl;

    public SupabaseStorageService(IAmazonS3 s3, IConfiguration cfg)
    {
        _s3 = s3;
        _bucket = cfg["Supabase:S3:Bucket"] ?? "memories-media";
        _baseUrl = (s3.Config as AmazonS3Config)?.ServiceURL?.TrimEnd('/') ?? "";
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct)
    {
        // важно: скопировать, чтобы исключить ObjectDisposed на внешнем потоке
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, ct);
        ms.Position = 0;

        var req = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = fileName,
            InputStream = ms,
            ContentType = contentType
        };
        // явно указываем длину
        req.Headers.ContentLength = ms.Length;

        await _s3.PutObjectAsync(req, ct);

        // публичный URL для path-style
        return string.IsNullOrEmpty(_baseUrl)
            ? $"/{_bucket}/{Uri.EscapeDataString(fileName)}"
            : $"{_baseUrl}/{_bucket}/{Uri.EscapeDataString(fileName)}";
    }

    public async Task DeleteAsync(string key, CancellationToken ct) =>
        await _s3.DeleteObjectAsync(_bucket, key, ct);

    public async Task<Stream> DownloadAsync(string key, CancellationToken ct)
    {
        var resp = await _s3.GetObjectAsync(_bucket, key, ct);
        var ms = new MemoryStream();
        await resp.ResponseStream.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }
}
