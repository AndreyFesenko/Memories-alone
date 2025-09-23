// src/MemoryArchiveService/MemoryArchiveService.API/Controllers/MemoryDownloadController.cs
using System.IO.Compression;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Queries; // GetMemoriesByUserQuery, GetMemoryByIdQuery

namespace MemoryArchiveService.API.Controllers;


[ApiController]
[Route("api/memory")]
public class MemoryDownloadController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;
    private readonly ILogger<MemoryDownloadController> _logger;

    public MemoryDownloadController(
        IMediator mediator,
        IAmazonS3 s3,
        IConfiguration cfg,
        ILogger<MemoryDownloadController> logger)
    {
        _mediator = mediator;
        _s3 = s3;
        _bucket = cfg["Supabase:S3:Bucket"] ?? "memories-media";
        _logger = logger;
    }

    // GET /memory/api/memory/user/{userId}/zip?accessLevel=&page=1&pageSize=50
    [HttpGet("user/{userId:guid}/zip")]
    [Authorize]
    public async Task<IActionResult> DownloadZipByUser(
        Guid userId,
        [FromQuery] string? accessLevel = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var paged = await _mediator.Send(new GetMemoriesByUserQuery
        {
            UserId = userId,
            AccessLevel = accessLevel,
            Page = page,
            PageSize = pageSize
        }, ct);

        return await BuildZipAsync(paged.Items, $"memories_{userId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip", ct);
    }

    // GET /memory/api/memory/{id}/zip
    [HttpGet("{id:guid}/zip")]
    [Authorize]
    public async Task<IActionResult> DownloadZipByMemory(Guid id, CancellationToken ct)
    {
        var mem = await _mediator.Send(new GetMemoryByIdQuery { Id = id }, ct);
        if (mem is null) return NotFound();

        return await BuildZipAsync(new[] { mem }, $"memory_{id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip", ct);
    }

    private async Task<IActionResult> BuildZipAsync(IEnumerable<MemoryDto> memories, string fileName, CancellationToken ct)
    {
        Response.ContentType = "application/zip";
        Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"";
        Response.StatusCode = (int)HttpStatusCode.OK;

        await using var responseStream = Response.Body;
        using var archive = new ZipArchive(responseStream, ZipArchiveMode.Create, leaveOpen: true);

        int index = 1;
        foreach (var mem in memories)
        {
            var safeTitle = Sanitize(mem.Title) ?? "memory";

            // корректная работа с null: AsEnumerable() || Enumerable.Empty<MediaFileDto>()
            var files = mem.MediaFiles?.AsEnumerable() ?? Enumerable.Empty<MediaFileDto>();

            foreach (var mf in files)
            {
                ct.ThrowIfCancellationRequested();

                var ext = Path.GetExtension(mf.FileName ?? string.Empty);
                if (string.IsNullOrWhiteSpace(ext))
                    ext = GuessExtensionByMediaType(mf.MediaType);
                var safeName = Sanitize(Path.GetFileNameWithoutExtension(mf.FileName) ?? mf.Id.ToString());
                var zipPath = $"{safeTitle}/{index:D3}_{safeName}{ext}";

                _logger.LogInformation("Preparing file {ZipPath} (mediaId={MediaId}, fileName={FileName})",
                    zipPath, mf.Id, mf.FileName);

                var key = TryExtractKey(mf);
                var added = false;
                if (!string.IsNullOrEmpty(key))
                {
                    try
                    {
                        _logger.LogInformation("Trying S3/presigned: bucket={Bucket}, key={Key}", _bucket, key);
                        await AddByS3OrSignedHttpAsync(archive, zipPath, key!, ct);
                        added = true; // внутри метода уже залогирован успех или фоллбек
                    }
                    catch (AmazonS3Exception s3ex) when (s3ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning("S3 not found: bucket={Bucket}, key={Key}. Will fallback to HTTP. {Message}",
                            _bucket, key, s3ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "S3/presigned failed for key {Key}. Will fallback to HTTP.", key);
                    }
                }

                if (!added)
                {
                    _logger.LogInformation("Fallback HTTP (public or presigned URL): url={Url}", mf.Url);
                    await AddByHttpDownloadAsync(archive, zipPath, mf.Url, ct);
                }

                index++;
            }
        }

        return new EmptyResult();
    }

    private async Task AddByS3OrSignedHttpAsync(ZipArchive archive, string zipPath, string key, CancellationToken ct)
    {
        try
        {
            // 1) Прямое чтение из приватного бакета
            var req = new GetObjectRequest { BucketName = _bucket, Key = key };
            using var obj = await _s3.GetObjectAsync(req, ct);
            var entry = archive.CreateEntry(zipPath, CompressionLevel.Fastest);
            await using var entryStream = entry.Open();
            await obj.ResponseStream.CopyToAsync(entryStream, ct);
            _logger.LogInformation("✔ S3 success: {ZipPath}", zipPath);
            return;
        }
        catch (AmazonS3Exception s3ex) when (
            s3ex.StatusCode == HttpStatusCode.Forbidden ||
            s3ex.StatusCode == HttpStatusCode.Unauthorized ||
            s3ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("S3 direct failed ({Code}) for key={Key}. Try presigned URL. {Msg}",
                s3ex.StatusCode, key, s3ex.Message);
        }

        // 2) Подписанная ссылка на 10 минут
        var pre = _s3.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(10)
        });

        await AddByHttpDownloadAsync(archive, zipPath, pre, ct);
    }

    private static async Task AddByHttpDownloadAsync(ZipArchive archive, string zipPath, string? url, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            var note = archive.CreateEntry(zipPath + ".failed.txt");
            await using var w = new StreamWriter(note.Open());
            await w.WriteAsync("No URL or key to fetch.");
            return;
        }

        using var http = new HttpClient();
        using var resp = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!resp.IsSuccessStatusCode)
        {
            var note = archive.CreateEntry(zipPath + ".failed.txt");
            await using var w = new StreamWriter(note.Open());
            await w.WriteAsync($"HTTP {(int)resp.StatusCode}: {resp.ReasonPhrase}");
            return;
        }

        var entry = archive.CreateEntry(zipPath, CompressionLevel.Fastest);
        await using var entryStream = entry.Open();
        await using var s = await resp.Content.ReadAsStreamAsync(ct);
        await s.CopyToAsync(entryStream, ct);
    }

    private string? TryExtractKey(MediaFileDto mf)
    {
        if (!string.IsNullOrWhiteSpace(mf.StorageUrl))
            return TrimLeadingSlash(mf.StorageUrl);

        var url = mf.Url ?? string.Empty;
        var noQs = url.Split('?', 2)[0];
        var marker = "/" + _bucket + "/";
        var idx = noQs.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return null;

        var tail = noQs[(idx + marker.Length)..];
        tail = Uri.UnescapeDataString(tail);
        return TrimLeadingSlash(tail);
    }

    private static string TrimLeadingSlash(string s) => s.StartsWith("/") ? s[1..] : s;

    private static string Sanitize(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "untitled";
        var s = name.Trim();
        foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
        return s.Length > 80 ? s[..80] : s;
    }

    private static string GuessExtensionByMediaType(string? mediaType) =>
        (mediaType?.ToLowerInvariant()) switch
        {
            "image" => ".jpg",
            "video" => ".mp4",
            "audio" => ".mp3",
            "document" => ".pdf",
            _ => ""
        };
}
