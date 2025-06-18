using MediatR;
namespace IdentityService.Application.Commands;

public class GetUsersByRoleQuery : IRequest<List<Guid>>
{
    public Guid RoleId { get; set; }
}
