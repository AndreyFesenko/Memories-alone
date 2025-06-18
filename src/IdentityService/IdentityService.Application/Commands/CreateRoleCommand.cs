using MediatR;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Commands;

public class CreateRoleCommand : IRequest<RoleDto>
{
    public string Name { get; set; } = default!;
}
