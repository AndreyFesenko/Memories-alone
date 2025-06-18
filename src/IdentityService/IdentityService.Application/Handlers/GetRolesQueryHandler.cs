using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Handlers;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IRoleRepository _roles;
    public GetRolesQueryHandler(IRoleRepository roles) => _roles = roles;

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken ct)
    {
        var roles = await _roles.GetAllAsync(ct);
        return roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name }).ToList();
    }
}
