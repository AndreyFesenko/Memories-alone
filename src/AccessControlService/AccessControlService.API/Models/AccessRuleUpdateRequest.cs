namespace AccessControlService.API.Models;

public class AccessRuleUpdateRequest
{
    public string? SubjectType { get; set; }
    public string? SubjectId { get; set; }

    public string? ResourceType { get; set; }
    public Guid? ResourceId { get; set; }

    public string? AccessType { get; set; }
    public string? Permission { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
