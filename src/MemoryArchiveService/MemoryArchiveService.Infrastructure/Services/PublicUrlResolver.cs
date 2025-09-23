// Infrastructure/Services/PublicUrlResolver.cs
using Microsoft.Extensions.Configuration;

namespace MemoryArchiveService.Infrastructure.Services;

public interface IPublicUrlResolver
{
    string? Resolve(string? url, string? storageUrl = null);
    string BuildFromKey(string key);
}

public sealed class SupabasePublicUrlResolver : IPublicUrlResolver
{
    private readonly string _bucket;
    private readonly string _publicBase;

    public SupabasePublicUrlResolver(IConfiguration cfg)
    {
        _bucket = cfg["Supabase:S3:Bucket"] ?? "memories-media";
        _publicBase = (cfg["Supabase:S3:PublicBaseUrl"] ?? "").TrimEnd('/');
    }

    public string BuildFromKey(string key)
        => $"{_publicBase}/{_bucket}/{key.TrimStart('/')}";

    public string? Resolve(string? url, string? storageUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(storageUrl))
        {
            // Если storageUrl — полный http-URL (/s3/), заменим префикс на public
            if (storageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return storageUrl.Replace("/storage/v1/s3/", "/storage/v1/object/public/");
            // Если это ключ — соберём public-URL
            return BuildFromKey(storageUrl);
        }

        if (string.IsNullOrWhiteSpace(url)) return null;
        // url от SDK: /s3/ → /object/public/
        return url.Replace("/storage/v1/s3/", "/storage/v1/object/public/");
    }
}
