using MediatR;
namespace IdentityService.Application.Commands;

public class UnassignRoleCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
