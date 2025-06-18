using MediatR;

namespace IdentityService.Application.Commands;

public class DeleteUserCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
    
}
