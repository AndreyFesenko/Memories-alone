namespace AuditLoggingService.Application.DTOs;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = default!;
    public string Details { get; set; } = default!;
    public Guid? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Result { get; set; }
}
