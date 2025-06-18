using IdentityService.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshTokenResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken);

	Task InvalidateAsync(string refreshToken, CancellationToken cancellationToken);

    Task RevokeAsync(Guid userId, string refreshToken, CancellationToken ct);
}
