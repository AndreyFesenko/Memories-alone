using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MemoryArchiveService.Application.Interfaces;

namespace MemoryArchiveService.Infrastructure.Services;

public sealed class MediaReadStore : IMediaReadStore
{
    private readonly MemoryArchiveDbContext _db;

    public MediaReadStore(MemoryArchiveDbContext db) => _db = db;

    public async Task<MediaFileDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.MediaFiles
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new MediaFileDto
            {
                Id = e.Id,
                MemoryId = e.MemoryId,
                FileName = e.FileName,
                Url = e.Url,
                StorageUrl = e.StorageUrl,
                MediaType = e.MediaType.ToString(),     // <-- фикс enum -> string
                Size = e.Size,
                UploadedAt = e.UploadedAt,
                CreatedAt = e.CreatedAt
            })
            .FirstOrDefaultAsync(ct);
    }
}
