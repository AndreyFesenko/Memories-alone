namespace IdentityService.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }

    /// <summary>
    /// �������� refresh-������
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ������ ������
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// ���� �������� ������
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// ����� �������� ������
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ����� ������ ������ (���� ���)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// �����, ������� ������� ���� (��� �������)
    /// </summary>
    public string? ReplacedByToken { get; set; }
}