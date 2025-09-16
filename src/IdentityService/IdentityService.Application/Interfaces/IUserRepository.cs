//C:\_C_Sharp\MyOtus_Prof\Memories_alone\src\IdentityService\IdentityService.Application\Interfaces\IUserRepository.cs
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken ct);
    Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task AddUserAsync(User user, CancellationToken ct);
    Task AddUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    Task<User?> FindByIdAsync(Guid userId, CancellationToken ct);
    Task UpdateUserAsync(User user, CancellationToken ct);
    Task DeleteUserAsync(Guid userId, CancellationToken ct);
    Task<List<Guid>> GetUsersByRoleAsync(Guid roleId, CancellationToken ct);
    Task RemoveUserRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    Task<User?> FindByEmailAsync(string email, CancellationToken ct);
 

}
