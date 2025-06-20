using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Domain.Entities;
using MemoryArchiveService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MemoryArchiveService.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly MemoryArchiveDbContext _db;

    public TagRepository(MemoryArchiveDbContext db)
    {
        _db = db;
    }

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _db.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);

    public async Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.Tags.ToListAsync(cancellationToken);

    public async Task<List<Tag>> SearchAsync(string? query = null, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var q = _db.Tags.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(t => t.Name.Contains(query));
        return await q
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? query = null, CancellationToken ct = default)
    {
        var q = _db.Tags.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(t => t.Name.Contains(query));
        return await q.CountAsync(ct);
    }

    public async Task AddAsync(Tag tag, CancellationToken ct = default)
    {
        await _db.Tags.AddAsync(tag, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Tag tag, CancellationToken ct = default)
    {
        _db.Tags.Update(tag);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tag != null)
        {
            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<List<Tag>> GetByMemoryIdAsync(Guid memoryId, CancellationToken ct = default)
    {
        return await _db.MemoryTags
            .Where(mt => mt.MemoryId == memoryId)
            .Include(mt => mt.Tag)
            .Select(mt => mt.Tag)
            .ToListAsync(ct);
    }

    public async Task AddTagToMemoryAsync(Guid memoryId, Guid tagId, CancellationToken ct = default)
    {
        var exists = await _db.MemoryTags.AnyAsync(mt => mt.MemoryId == memoryId && mt.TagId == tagId, ct);
        if (!exists)
        {
            _db.MemoryTags.Add(new MemoryTag { MemoryId = memoryId, TagId = tagId });
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveTagFromMemoryAsync(Guid memoryId, Guid tagId, CancellationToken ct = default)
    {
        var link = await _db.MemoryTags.FirstOrDefaultAsync(mt => mt.MemoryId == memoryId && mt.TagId == tagId, ct);
        if (link != null)
        {
            _db.MemoryTags.Remove(link);
            await _db.SaveChangesAsync(ct);
        }
    }
}
