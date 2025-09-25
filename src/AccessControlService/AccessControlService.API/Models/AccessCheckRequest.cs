namespace AccessControlService.API.Models;

public class AccessCheckRequest
{
    public string SubjectId { get; set; } = default!;
    public string ResourceType { get; set; } = "Memory";
    public Guid ResourceId { get; set; }
    public string? SubjectType { get; set; } // "User" | "Role"
    public string? AccessType { get; set; }
    public string? Permission { get; set; }
}
