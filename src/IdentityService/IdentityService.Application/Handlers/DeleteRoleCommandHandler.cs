using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Handlers;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Unit>
{
    private readonly IRoleRepository _roles;
    public DeleteRoleCommandHandler(IRoleRepository roles) => _roles = roles;

    public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        await _roles.DeleteAsync(request.RoleId, ct);
        return Unit.Value;
    }
}
