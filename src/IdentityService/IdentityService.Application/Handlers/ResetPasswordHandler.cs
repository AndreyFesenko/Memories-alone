using MediatR;
using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IUserRepository _users;
    public ResetPasswordHandler(IUserRepository users) => _users = users;

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(request.Email, ct);
        if (user == null || user.PasswordResetToken != request.Token)
            throw new Exception("Invalid token");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetRequestedAt = null;
        await _users.UpdateUserAsync(user, ct);

        return Unit.Value;
    }
}
