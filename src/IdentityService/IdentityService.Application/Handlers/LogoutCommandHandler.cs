using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Handlers;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenService _refreshTokens;
    public LogoutCommandHandler(IRefreshTokenService refreshTokens) => _refreshTokens = refreshTokens;

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken ct)
    {
        await _refreshTokens.RevokeAsync(request.UserId, request.RefreshToken, ct);
        return Unit.Value;
    }
}