using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using MediatR;

namespace IdentityService.Application.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IAuditService _audit;

    public RegisterCommandHandler(IUserRepository users, IRoleRepository roles, IAuditService audit)
    {
        _users = users;
        _roles = roles;
        _audit = audit;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _users.EmailExistsAsync(request.Email, cancellationToken))
            throw new InvalidOperationException("Пользователь с таким email уже существует");

        // UserName обязательно! Если не передали — использовать email
        var userName = !string.IsNullOrWhiteSpace(request.UserName) ? request.UserName : request.Email;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = userName, // <--- ключевая строка!
            NormalizedUserName = request.Email.ToUpperInvariant(),
            NormalizedEmail = request.Email.ToUpperInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddUserAsync(user, cancellationToken);

        var defaultRoleId = await _roles.GetRoleIdByNameAsync("RegularUser", cancellationToken);
        await _users.AddUserRoleAsync(user.Id, defaultRoleId, cancellationToken);

        // 📋 Логируем регистрацию
        await _audit.LogAsync(
            "Register",
            $"Зарегистрирован пользователь {user.Email}",
            user.Id,
            cancellationToken
        );

        return new RegisterResponse(user.Id, user.Email);
    }
}
