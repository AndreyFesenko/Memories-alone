using MediatR;

namespace IdentityService.Application.Commands;

public class RequestPasswordResetCommand : IRequest<Unit>
{
    public string Email { get; set; } = default!;
}
