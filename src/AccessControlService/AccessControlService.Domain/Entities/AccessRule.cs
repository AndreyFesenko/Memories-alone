namespace AccessControlService.Domain.Entities;

public class AccessRule
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }     // UserId/roleId
    public Guid ObjectId { get; set; }      // ResourceId
    public string AccessType { get; set; } = default!; // read/write/admin
    public string? GrantedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
