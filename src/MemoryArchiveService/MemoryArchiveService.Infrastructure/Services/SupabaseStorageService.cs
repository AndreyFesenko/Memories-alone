// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Services/SupabaseStorageService.cs
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using MemoryArchiveService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MemoryArchiveService.Infrastructure.Services
{
    /// <summary>
    /// S3-совместимое хранилище Supabase.
    /// Работает через IAmazonS3, ServiceURL берётся из конфигурации.
    /// </summary>
    public class SupabaseStorageService : IStorageService
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucket;
        private readonly string _serviceUrl; // например: https://...supabase.co/storage/v1/s3

        public SupabaseStorageService(IAmazonS3 s3, IConfiguration cfg)
        {
            _s3 = s3 ?? throw new ArgumentNullException(nameof(s3));
            _bucket = cfg["Supabase:S3:Bucket"] ?? throw new InvalidOperationException("Supabase:S3:Bucket is missing");
            _serviceUrl = cfg["Supabase:S3:Endpoint"] ?? throw new InvalidOperationException("Supabase:S3:Endpoint is missing");
        }

        public async Task<UploadResult> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            // гарантируем, что читаем с начала
            if (stream.CanSeek) stream.Position = 0;

            var put = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = fileName,
                InputStream = stream,
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
                AutoCloseStream = false // не закрываем внешний поток принудительно
            };

            var putResp = await _s3.PutObjectAsync(put, ct);

            // Забираем метаданные, чтобы узнать точный ContentLength/ETag
            var meta = await _s3.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                BucketName = _bucket,
                Key = fileName
            }, ct);

            var url = BuildPublicUrl(fileName);
            var size = meta.Headers.ContentLength;
            var etag = putResp.ETag;

            return new UploadResult(url, size, etag);
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

        private string BuildPublicUrl(string key)
        {
            // Для Supabase S3 (path-style): <ServiceURL>/<bucket>/<key>
            // Пример: https://.../storage/v1/s3/memories-media/my/photo.jpg
            var baseUrl = _serviceUrl.TrimEnd('/');
            var escapedKey = Uri.EscapeDataString(key).Replace("%2F", "/"); // сохраняем слеши в ключе
            return $"{baseUrl}/{_bucket}/{escapedKey}";
        }
    }
}
