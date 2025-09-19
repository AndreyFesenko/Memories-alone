// src/MemoryArchiveService/MemoryArchiveService.Application/Interfaces/IStorageService.cs
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryArchiveService.Application.Interfaces
{
    /// <summary>
    /// Результат загрузки файла в хранилище.
    /// </summary>
    public sealed record UploadResult(string Url, long Size, string? ETag);

    public interface IStorageService
    {
        /// <summary>
        /// Загружает поток в объектное хранилище.
        /// Возвращает публичный (или адрес для доступа) URL, реальный Size и ETag (если есть).
        /// </summary>
        Task<UploadResult> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);

        Task<Stream> DownloadAsync(string fileName, CancellationToken ct = default);

        Task DeleteAsync(string fileName, CancellationToken ct = default);
    }
}
