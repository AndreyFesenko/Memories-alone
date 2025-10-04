using MediatR;

namespace ProfileService.Application.Commands;

public class ConfirmDeathCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
}
