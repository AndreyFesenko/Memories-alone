namespace AccessControlService.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public string Action { get; set; } = default!;
    public string Details { get; set; } = default!;
    public Guid? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
