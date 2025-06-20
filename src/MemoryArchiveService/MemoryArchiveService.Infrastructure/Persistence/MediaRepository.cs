using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Domain.Entities;
using MemoryArchiveService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MemoryArchiveService.Infrastructure.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly MemoryArchiveDbContext _db;

    public MediaRepository(MemoryArchiveDbContext db)
    {
        _db = db;
    }

    public async Task<MediaFile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.MediaFiles.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<List<MediaFile>> GetByMemoryIdAsync(Guid memoryId, CancellationToken ct = default)
        => await _db.MediaFiles.Where(f => f.MemoryId == memoryId).ToListAsync(ct);

    public async Task<List<MediaFile>> SearchAsync(
        Guid? memoryId = null,
        MediaType? mediaType = null,
        string? ownerId = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var q = _db.MediaFiles.AsQueryable();

        if (memoryId.HasValue)
            q = q.Where(x => x.MemoryId == memoryId);

        if (mediaType.HasValue)
            q = q.Where(x => x.MediaType == mediaType.Value);

        if (!string.IsNullOrWhiteSpace(ownerId))
            q = q.Where(x => x.OwnerId == ownerId);

        return await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(
        Guid? memoryId = null,
        MediaType? mediaType = null,
        string? ownerId = null,
        CancellationToken ct = default)
    {
        var q = _db.MediaFiles.AsQueryable();

        if (memoryId.HasValue)
            q = q.Where(x => x.MemoryId == memoryId);

        if (mediaType.HasValue)
            q = q.Where(x => x.MediaType == mediaType.Value);

        if (!string.IsNullOrWhiteSpace(ownerId))
            q = q.Where(x => x.OwnerId == ownerId);

        return await q.CountAsync(ct);
    }

    public async Task AddAsync(MediaFile file, CancellationToken ct = default)
    {
        await _db.MediaFiles.AddAsync(file, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(MediaFile file, CancellationToken ct = default)
    {
        _db.MediaFiles.Update(file);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var file = await _db.MediaFiles.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (file != null)
        {
            _db.MediaFiles.Remove(file);
            await _db.SaveChangesAsync(ct);
        }
    }
}
