using MediatR;
using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;

namespace IdentityService.Application.Handlers;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<string>>
{
    private readonly IUserRepository _users;

    public GetUserRolesQueryHandler(IUserRepository users) => _users = users;

    public async Task<List<string>> Handle(GetUserRolesQuery request, CancellationToken ct)
        => (await _users.GetUserRolesAsync(request.UserId, ct)).ToList();
}
