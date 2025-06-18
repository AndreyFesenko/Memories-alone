using MediatR;
using IdentityService.Application.Commands;
using IdentityService.Application.Interfaces;

public class RequestEmailConfirmationHandler : IRequestHandler<RequestEmailConfirmationCommand, Unit>
{
    private readonly IUserRepository _users;
    public RequestEmailConfirmationHandler(IUserRepository users) => _users = users;

    public async Task<Unit> Handle(RequestEmailConfirmationCommand request, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(request.Email, ct);
        if (user == null) throw new Exception("Not found");

        user.EmailConfirmationToken = Guid.NewGuid().ToString("N");
        await _users.UpdateUserAsync(user, ct);

        // Для MVP — вывести токен (в проде отправлять email)
        Console.WriteLine($"CONFIRM LINK: /api/Auth/confirm-email?email={user.Email}&token={user.EmailConfirmationToken}");

        return Unit.Value;
    }
}
