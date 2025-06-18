using MediatR;

namespace IdentityService.Application.Commands;

/// <summary>
/// Команда для смены пароля пользователя
/// </summary>
public class ChangePasswordCommand : IRequest
{
    public Guid UserId { get; set; }
    public string OldPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}
