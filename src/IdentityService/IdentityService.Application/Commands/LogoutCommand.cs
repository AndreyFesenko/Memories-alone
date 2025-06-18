using MediatR;

namespace IdentityService.Application.Commands;

public class LogoutCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = default!;
}
