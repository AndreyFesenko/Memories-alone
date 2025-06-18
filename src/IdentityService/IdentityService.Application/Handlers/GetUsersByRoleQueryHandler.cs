using MediatR;
using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;

namespace IdentityService.Application.Handlers;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, List<Guid>>
{
    private readonly IUserRepository _users;
    public GetUsersByRoleQueryHandler(IUserRepository users) => _users = users;

    public async Task<List<Guid>> Handle(GetUsersByRoleQuery request, CancellationToken ct)
        => await _users.GetUsersByRoleAsync(request.RoleId, ct);
}
