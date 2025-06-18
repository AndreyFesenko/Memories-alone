using MediatR;
using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;

public class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, Unit>
{
    private readonly IUserRepository _users;
    public RequestPasswordResetHandler(IUserRepository users) => _users = users;

    public async Task<Unit> Handle(RequestPasswordResetCommand request, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(request.Email, ct);
        if (user == null) return Unit.Value;

        user.PasswordResetToken = Guid.NewGuid().ToString("N");
        user.PasswordResetRequestedAt = DateTime.UtcNow;
        await _users.UpdateUserAsync(user, ct);

        Console.WriteLine($"RESET LINK: /api/Auth/reset-password?email={user.Email}&token={user.PasswordResetToken}");

        return Unit.Value;
    }
}
