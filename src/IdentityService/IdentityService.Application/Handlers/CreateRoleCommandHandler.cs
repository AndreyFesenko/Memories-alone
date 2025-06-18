using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using MediatR;

namespace IdentityService.Application.Handlers;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
{
    private readonly IRoleRepository _roles;
    public CreateRoleCommandHandler(IRoleRepository roles) => _roles = roles;

    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var role = new Role { Id = Guid.NewGuid(), Name = request.Name };
        await _roles.AddAsync(role, ct);
        return new RoleDto { Id = role.Id, Name = role.Name };
    }
}
