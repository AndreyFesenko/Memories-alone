// src/MemoryArchiveService/MemoryArchiveService.Application/Interfaces/IMediaRepository.cs
using MemoryArchiveService.Domain.Entities;

namespace MemoryArchiveService.Application.Interfaces;

public interface IMediaRepository
{
    Task<MediaFile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<MediaFile>> GetByMemoryIdAsync(Guid memoryId, CancellationToken ct = default);

    // Используем MediaType? вместо string? fileType
    Task<List<MediaFile>> SearchAsync(
        Guid? memoryId = null,
        MediaType? mediaType = null,
        string? ownerId = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<int> CountAsync(
        Guid? memoryId = null,
        MediaType? mediaType = null,
        string? ownerId = null,
        CancellationToken ct = default);

    Task AddAsync(MediaFile file, CancellationToken ct = default);
    Task UpdateAsync(MediaFile file, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

