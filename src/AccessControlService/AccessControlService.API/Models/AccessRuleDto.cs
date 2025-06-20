namespace AccessControlService.API.Models;

public class AccessRuleDto
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ObjectId { get; set; }
    public string AccessType { get; set; } = default!;
    public string? GrantedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
