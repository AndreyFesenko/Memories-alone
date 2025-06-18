using IdentityService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Unit>
{
    private readonly IUserRepository _users;
    public AssignRoleCommandHandler(IUserRepository users) => _users = users;

    public async Task<Unit> Handle(AssignRoleCommand request, CancellationToken ct)
    {
        await _users.AddUserRoleAsync(request.UserId, request.RoleId, ct);
        return Unit.Value;
    }
}