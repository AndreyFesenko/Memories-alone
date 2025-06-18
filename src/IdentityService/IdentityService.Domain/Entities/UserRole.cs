namespace IdentityService.Domain.Entities;

public class UserRole
{
    /// <summary>
    /// Id ������������
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ������������� �������� � ������������
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Id ����
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// ������������� �������� � ����
    /// </summary>
    public Role Role { get; set; } = null!;
}