using MediatR;
using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;

namespace IdentityService.Application.Handlers;

public class UnassignRoleCommandHandler : IRequestHandler<UnassignRoleCommand, Unit>
{
    private readonly IUserRepository _users;
    public UnassignRoleCommandHandler(IUserRepository users) => _users = users;

    public async Task<Unit> Handle(UnassignRoleCommand request, CancellationToken ct)
    {
        await _users.RemoveUserRoleAsync(request.UserId, request.RoleId, ct);
        return Unit.Value;
    }
}
