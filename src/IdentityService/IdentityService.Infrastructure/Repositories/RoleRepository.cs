using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly MemoriesDbContext _dbContext;
    public RoleRepository(MemoriesDbContext dbContext) => _dbContext = dbContext;

    public async Task<List<Role>> GetAllAsync(CancellationToken ct)
        => await _dbContext.Roles.ToListAsync(ct);

    public async Task AddAsync(Role role, CancellationToken ct)
    {
        //  гарантируем заполнение normalized
        if (!string.IsNullOrWhiteSpace(role.Name) && string.IsNullOrWhiteSpace(role.NormalizedName))
            role.NormalizedName = role.Name.ToUpperInvariant();

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<Guid> GetRoleIdByNameAsync(string roleName, CancellationToken ct) =>
        await _dbContext.Roles
            .Where(r => r.NormalizedName == roleName.ToUpperInvariant()) // ищем по normalized
            .Select(r => r.Id)
            .FirstAsync(ct);

    public async Task DeleteAsync(Guid roleId, CancellationToken ct)
    {
        var role = await _dbContext.Roles.FindAsync(new object[] { roleId }, ct);
        if (role != null)
        {
            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<Role> CreateAsync(string name, CancellationToken ct)
    {
        var norm = (name ?? "").Trim().ToUpperInvariant();

        // если роль уже есть – вернуть её
        var existing = await _dbContext.Roles.FirstOrDefaultAsync(r => r.NormalizedName == norm, ct);
        if (existing is not null) return existing;

        var role = new Role { Id = Guid.NewGuid(), Name = name.Trim(), NormalizedName = norm };
        _dbContext.Roles.Add(role);
        try
        {
            await _dbContext.SaveChangesAsync(ct);
            return role;
        }
        catch (DbUpdateException)
        {
            // на случай гонки: повторно читаем
            var r = await _dbContext.Roles.FirstAsync(r => r.NormalizedName == norm, ct);
            return r;
        }
    }

}
