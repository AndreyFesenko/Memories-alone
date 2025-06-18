namespace IdentityService.Domain.Entities;

public class UserRole
{
    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Навигационное свойство — пользователь
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Id роли
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Навигационное свойство — роль
    /// </summary>
    public Role Role { get; set; } = null!;
}