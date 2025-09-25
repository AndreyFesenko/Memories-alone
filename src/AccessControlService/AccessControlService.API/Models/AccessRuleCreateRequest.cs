namespace AccessControlService.API.Models;

public class AccessRuleCreateRequest
{
    public string SubjectType { get; set; } = "User"; // optional
    public string SubjectId { get; set; } = default!; // required

    public string ResourceType { get; set; } = "Memory"; // optional
    public Guid ResourceId { get; set; }                 // required

    // можно прислать и как AccessType, и как Permission — берём любое
    public string? AccessType { get; set; }
    public string? Permission { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
