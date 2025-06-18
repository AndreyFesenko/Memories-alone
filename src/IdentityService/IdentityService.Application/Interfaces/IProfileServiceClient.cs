using IdentityService.Application.DTOs;

namespace IdentityService.Application.Interfaces;

public interface IProfileServiceClient
{
    Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
}
