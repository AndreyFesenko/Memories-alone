//C:\_C_Sharp\MyOtus_Prof\Memories_alone\src\IdentityService\IdentityService.Infrastructure\Repositories\UserRepository.cs
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MemoriesDbContext _db;

    public UserRepository(MemoriesDbContext db) => _db = db;

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        await _db.Users.AnyAsync(u => u.NormalizedEmail == email.ToUpperInvariant(), ct);

    public async Task<List<User>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Users.ToListAsync(ct);
    }

    public async Task AddUserAsync(User user, CancellationToken ct)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        _db.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateUserAsync(User user, CancellationToken ct)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, ct);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync(ct);
        }
    }
    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);
    }

    public async Task<List<Guid>> GetUsersByRoleAsync(Guid roleId, CancellationToken ct)
    {
        return await _db.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync(ct);
    }

    public async Task RemoveUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        var userRole = await _db.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);

        if (userRole != null)
        {
            _db.UserRoles.Remove(userRole);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<User?> FindByIdAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);
    }



}
