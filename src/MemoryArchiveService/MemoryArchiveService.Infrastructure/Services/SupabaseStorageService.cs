using Amazon.S3;
using Amazon.S3.Transfer;
using MemoryArchiveService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MemoryArchiveService.Infrastructure.Services;

public class SupabaseStorageService : IStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;

    public SupabaseStorageService(IAmazonS3 s3, IConfiguration config)
    {
        _s3 = s3;
        _bucket = config["Supabase:S3:Bucket"];
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = stream,
            Key = fileName,
            BucketName = _bucket,
            ContentType = contentType
        };

        var transfer = new TransferUtility(_s3);
        await transfer.UploadAsync(uploadRequest, ct);

        return $"https://{_bucket}.supabase.co/storage/v1/object/public/{fileName}";
    }
}
