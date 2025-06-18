using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;
using MediatR;

namespace IdentityService.Application.Handlers;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _users;
    private readonly IAuditService _audit;

    public ChangePasswordCommandHandler(IUserRepository users, IAuditService audit)
    {
        _users = users;
        _audit = audit;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.FindByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new Exception("User not found");

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Wrong current password");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _users.UpdateUserAsync(user, cancellationToken);

        await _audit.LogAsync(
            "ChangePassword",
            $"Пользователь {user.Email} сменил пароль",
            user.Id,
            cancellationToken
        );
    }
}
