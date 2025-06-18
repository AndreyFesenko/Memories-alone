using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;
using MediatR;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _users;
    private readonly IAuditService _audit;

    public DeleteUserCommandHandler(IUserRepository users, IAuditService audit)
    {
        _users = users;
        _audit = audit;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.FindByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new Exception("User not found");

        await _users.DeleteUserAsync(user.Id, cancellationToken);

        // 📋 Логируем удаление
        await _audit.LogAsync(
            "DeleteUser",
            $"Пользователь {user.Email} удалён администратором",
            request.AdminId, // сюда можно подставить ID админа
            cancellationToken
        );

        return Unit.Value;
    }
}
