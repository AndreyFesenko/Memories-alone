using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IProfileServiceClient _profileClient;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly IAuditService _audit;

    public LoginCommandHandler(
        IUserRepository users,
        IJwtTokenGenerator jwt,
        IProfileServiceClient profileClient,
        ILogger<LoginCommandHandler> logger,
        IAuditService audit)
    {
        _users = users;
        _jwt = jwt;
        _profileClient = profileClient;
        _logger = logger;
        _audit = audit;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("������� ����� ��� ������������ {Email}", request.Email);

        var user = await _users.FindByEmailAsync(request.Email, cancellationToken);

        // �������� ����������
        if (user != null && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            _logger.LogWarning("���� ������������: ������������ {Email} �� {LockoutEnd}", request.Email, user.LockoutEnd);
            await _audit.LogAsync("LoginBlocked", $"���� ������������: ������������ {request.Email} �� {user.LockoutEnd}", user.Id, cancellationToken);
            throw new UnauthorizedAccessException("Account is locked. Try again later.");
        }

        // �������� ������������ � ������
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            if (user != null)
            {
                user.AccessFailedCount += 1;
                // ���������� ����� 5 ��������� ������� �� 15 �����
                if (user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.AccessFailedCount = 0;
                    _logger.LogWarning("������������ {Email} ������������ �� {LockoutEnd} ����� 5 ��������� �������.", request.Email, user.LockoutEnd);
                    await _audit.LogAsync("LoginLockout", $"������������ {user.Email} ������������ �� {user.LockoutEnd} ����� 5 ��������� �������.", user.Id, cancellationToken);
                }
                await _users.UpdateUserAsync(user, cancellationToken);
            }

            _logger.LogWarning("������ �����: ������������ email ��� ������ ��� {Email}", request.Email);
            await _audit.LogAsync("LoginFailed", $"��������� ���� ��� {request.Email}", user?.Id, cancellationToken);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // ����� �������� � ���������� ��� �������� �����
        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
        await _users.UpdateUserAsync(user, cancellationToken);

        await _audit.LogAsync("Login", $"������������ {user.Email} ����� �������", user.Id, cancellationToken);

        var roles = await _users.GetUserRolesAsync(user.Id, cancellationToken);

        // ���������� � ProfileService ��� �������������� claims
        var profile = await _profileClient.GetProfileAsync(user.Id, cancellationToken);

        var customClaims = new Dictionary<string, string>
        {
            { "AccessMode", profile?.AccessMode ?? "AfterDeath" },
            { "DeathConfirmed", profile?.DeathConfirmed == true ? "true" : "false" }
        };

        var token = _jwt.GenerateToken(user.Id, user.Email, roles, customClaims);

        // ��������� refresh token � ������ (�������� �������� ���������!)
        var refreshToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

        _logger.LogInformation("�������� ���� ��� ������������ {Email} c ������: {Roles}", request.Email, string.Join(",", roles));

        return new LoginResponse(token, refreshToken);
    }
}
