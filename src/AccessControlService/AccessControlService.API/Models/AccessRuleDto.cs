namespace AccessControlService.API.Models;

public class AccessRuleDto
{
    public Guid Id { get; set; }

    // кто получает доступ
    public string SubjectType { get; set; } = "User"; // "User" | "Role"
    public string SubjectId { get; set; } = default!; // userId или имя роли

    // к чему доступ
    public string ResourceType { get; set; } = "Memory"; // "Memory"
    public Guid ResourceId { get; set; }                 // memoryId

    // что можно
    public string Permission { get; set; } = "View";     // "View" | "Edit" | "Delete"

    public DateTime? ExpiresAt { get; set; }

    // сервисные поля
    public Guid? GrantedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
