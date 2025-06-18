using MediatR;

namespace IdentityService.Application.Commands;

public class DeleteRoleCommand : IRequest<Unit>
{
    public Guid RoleId { get; set; }
}