// src/MemoryArchiveService/MemoryArchiveService.Application/Interfaces/ITagRepository.cs
using MemoryArchiveService.Domain.Entities;

namespace MemoryArchiveService.Application.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Tag>> SearchAsync(
        string? query = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? query = null, CancellationToken ct = default);
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Tag tag, CancellationToken ct = default);
    Task UpdateAsync(Tag tag, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Получить все теги, привязанные к MemoryId
    /// </summary>
    Task<List<Tag>> GetByMemoryIdAsync(Guid memoryId, CancellationToken ct = default);

    /// <summary>
    /// Привязать тег к MemoryId
    /// </summary>
    Task AddTagToMemoryAsync(Guid memoryId, Guid tagId, CancellationToken ct = default);

    /// <summary>
    /// Убрать тег у MemoryId
    /// </summary>
    Task RemoveTagFromMemoryAsync(Guid memoryId, Guid tagId, CancellationToken ct = default);
}
