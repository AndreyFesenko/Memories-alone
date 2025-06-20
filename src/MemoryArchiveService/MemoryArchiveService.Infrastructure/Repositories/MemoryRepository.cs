// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Repositories/MemoryRepository.cs
using MemoryArchiveService.Domain.Entities;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MemoryArchiveService.Infrastructure.Repositories;

public class MemoryRepository : IMemoryRepository
{
    private readonly MemoryArchiveDbContext _db;
    public MemoryRepository(MemoryArchiveDbContext db) => _db = db;

    public async Task<Memory?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.Memories.Include(m => m.MediaFiles).Include(m => m.Tags)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<List<Memory>> GetByOwnerAsync(Guid ownerId, CancellationToken ct) =>
        await _db.Memories.Include(m => m.MediaFiles).Include(m => m.Tags)
            .Where(m => m.OwnerId == ownerId).ToListAsync(ct);

    public async Task AddAsync(Memory memory, CancellationToken ct)
    {
        _db.Memories.Add(memory);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Memory memory, CancellationToken ct)
    {
        _db.Memories.Update(memory);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var memory = await _db.Memories.FindAsync(new object[] { id }, ct);
        if (memory != null)
        {
            _db.Memories.Remove(memory);
            await _db.SaveChangesAsync(ct);
        }
    }
}
