namespace ProfileService.Application.Interfaces;

using ProfileService.Domain.Entities;

public interface IProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<UserProfile>> GetAllAsync(CancellationToken ct);
    Task AddAsync(UserProfile profile, CancellationToken ct);
    Task UpdateAsync(UserProfile profile, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<UserProfile> CreateAsync(UserProfile profile, CancellationToken ct);
}
