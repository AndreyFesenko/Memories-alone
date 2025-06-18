using MediatR;
namespace IdentityService.Application.Commands;

public class GetUserRolesQuery : IRequest<List<string>>
{
    public Guid UserId { get; set; }
}
