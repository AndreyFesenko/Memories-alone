namespace AccessControlService.API.Models;

public class AccessRevokeRequest
{
    public Guid SubjectId { get; set; }
    public Guid ObjectId { get; set; }
    public string AccessType { get; set; } = "read";
}
