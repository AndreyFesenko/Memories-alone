namespace AccessControlService.API.Models;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = default!;
    public string? Details { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
