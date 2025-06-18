namespace IdentityService.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }

    /// <summary>
    /// Владелец refresh-токена
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Строка токена
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Срок действия токена
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Время создания токена
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Время отзыва токена (если был)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Токен, который заменил этот (при ротации)
    /// </summary>
    public string? ReplacedByToken { get; set; }
}