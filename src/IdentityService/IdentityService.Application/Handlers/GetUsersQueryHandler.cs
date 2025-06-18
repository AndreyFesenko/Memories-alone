using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Handlers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _users;

    public GetUsersQueryHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _users.GetAllAsync(cancellationToken);
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var userRoles = await _users.GetUserRolesAsync(user.Id, cancellationToken);
            result.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmedAt != null,
                Roles = userRoles.ToList()
            });
        }
        return result;
    }
}
