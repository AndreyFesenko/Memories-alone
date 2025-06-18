using IdentityService.Application.DTOs;
using MediatR;

public class AssignRoleCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
