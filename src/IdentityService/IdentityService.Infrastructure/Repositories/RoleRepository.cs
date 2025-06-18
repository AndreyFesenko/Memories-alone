using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class RoleRepository : IRoleRepository
{
    private readonly MemoriesDbContext _dbContext;
    public RoleRepository(MemoriesDbContext dbContext) => _dbContext = dbContext;

    public async Task<List<Role>> GetAllAsync(CancellationToken ct)
        => await _dbContext.Roles.ToListAsync(ct);

    public async Task AddAsync(Role role, CancellationToken ct)
    {
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<Guid> GetRoleIdByNameAsync(string roleName, CancellationToken ct) =>
        await _dbContext.Roles.Where(r => r.Name == roleName).Select(r => r.Id).FirstAsync(ct);
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
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = name
        };
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(ct);
        return role;
    }
}

