using MediatR;
using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Unit>
{
    private readonly IUserRepository _users;
    public ConfirmEmailHandler(IUserRepository users) => _users = users;

    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(request.Email, ct);
        if (user == null || user.EmailConfirmationToken != request.Token)
            throw new Exception("Invalid token");

        user.EmailConfirmedAt = DateTime.UtcNow;
        user.EmailConfirmationToken = null;
        await _users.UpdateUserAsync(user, ct);

        return Unit.Value;
    }
}
