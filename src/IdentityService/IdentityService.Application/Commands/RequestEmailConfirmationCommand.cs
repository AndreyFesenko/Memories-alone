using MediatR;

namespace IdentityService.Application.Commands;

public class RequestEmailConfirmationCommand : IRequest<Unit>
{
    public string Email { get; set; } = default!;
}
