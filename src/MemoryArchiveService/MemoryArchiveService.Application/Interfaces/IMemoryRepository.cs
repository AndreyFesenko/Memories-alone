// src/MemoryArchiveService/MemoryArchiveService.Application/Interfaces/IMemoryRepository.cs
using MemoryArchiveService.Domain.Entities;

namespace MemoryArchiveService.Application.Interfaces;

public interface IMemoryRepository
{
    Task<Memory?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Memory>> GetByOwnerAsync(Guid ownerId, CancellationToken ct);
    Task AddAsync(Memory memory, CancellationToken ct);
    Task UpdateAsync(Memory memory, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);

    //Task SaveChangesAsync(CancellationToken ct);
}
