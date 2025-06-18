using MediatR;

namespace IdentityService.Application.Commands;

public class ConfirmEmailCommand : IRequest<Unit>
{
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}
