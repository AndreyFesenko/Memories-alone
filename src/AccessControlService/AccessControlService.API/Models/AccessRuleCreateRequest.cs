namespace AccessControlService.API.Models;

public class AccessRuleCreateRequest
{
    public Guid SubjectId { get; set; }
    public Guid ObjectId { get; set; }
    public string AccessType { get; set; } = default!;
    public string? GrantedBy { get; set; }
}
