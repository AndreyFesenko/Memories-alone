using Microsoft.EntityFrameworkCore;
using ProfileService.Application.Interfaces;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ProfilesDbContext _db;

    public ProfileRepository(ProfilesDbContext db)
    {
        _db = db;
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<UserProfile>> GetAllAsync(CancellationToken ct)
        => await _db.UserProfiles.AsNoTracking().ToListAsync(ct);

    public async Task<UserProfile> CreateAsync(UserProfile profile, CancellationToken ct)
    {
        profile.Id = profile.Id == Guid.Empty ? Guid.NewGuid() : profile.Id;
        profile.CreatedAt = DateTime.UtcNow;

        _db.UserProfiles.Add(profile);
        await _db.SaveChangesAsync(ct);
        return profile;
    }

    public async Task UpdateAsync(UserProfile profile, CancellationToken ct)
    {
        profile.UpdatedAt = DateTime.UtcNow;
        _db.UserProfiles.Update(profile);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _db.UserProfiles.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return;
        _db.UserProfiles.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}
