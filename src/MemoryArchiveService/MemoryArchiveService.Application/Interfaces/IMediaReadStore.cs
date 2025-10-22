using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Interfaces;

public interface IMediaReadStore
{
    Task<MediaFileDto?> GetByIdAsync(Guid id, CancellationToken ct);
}
