//C:\_C_Sharp\MyOtus_Prof\Memories_alone\src\IdentityService\IdentityService.Application\Interfaces\IRoleRepository.cs
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces;

public interface IRoleRepository
{
    Task<Guid> GetRoleIdByNameAsync(string roleName, CancellationToken ct);
    Task<List<Role>> GetAllAsync(CancellationToken ct);
    Task AddAsync(Role role, CancellationToken ct);
    Task DeleteAsync(Guid roleId, CancellationToken ct);
    Task<Role> CreateAsync(string name, CancellationToken ct);
}
