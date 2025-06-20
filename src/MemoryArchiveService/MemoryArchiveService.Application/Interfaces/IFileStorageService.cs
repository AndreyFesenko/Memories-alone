// Application/Interfaces/IFileStorageService.cs
public interface IFileStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct);
    // ...DownloadAsync, DeleteAsync
}
