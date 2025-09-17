// src\MemoryArchiveService\MemoryArchiveService.Application\Interfaces\IMediaStorageService.cs
using System.Threading;
using System.Threading.Tasks;

namespace MemoryArchiveService.Application.Interfaces;

public interface IMediaStorageService
{
    Task<string> UploadAsync(string fileName, Stream fileStream, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string fileName, CancellationToken ct = default);
    Task DeleteAsync(string fileName, CancellationToken ct = default);
}
