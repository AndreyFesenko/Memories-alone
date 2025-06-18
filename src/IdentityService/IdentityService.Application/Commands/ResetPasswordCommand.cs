using MediatR;

namespace IdentityService.Application.Commands;

public class ResetPasswordCommand : IRequest<Unit>
{
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}
