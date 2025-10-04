using ProfileService.Domain.Entities;

namespace ProfileService.Application.Interfaces;

public interface IProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<UserProfile>> GetAllAsync(CancellationToken ct);
    Task<UserProfile> CreateAsync(UserProfile profile, CancellationToken ct);
    Task UpdateAsync(UserProfile profile, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
