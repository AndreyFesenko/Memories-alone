namespace IdentityService.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }

    /// <summary>
    /// ��� ���� (��������: "AccountHolder")
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// ��� ���� � ������� �������� ��� ������/������������ (��������: "ACCOUNTHOLDER")
    /// </summary>
    public string NormalizedName { get; set; } = null!;

    /// <summary>
    /// �������� ����
    /// </summary>
    public string? Description { get; set; }
}