namespace AuditLoggingService.Application.DTOs;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = default!;
    public string Target { get; set; } = default!;
    public string? Data { get; set; }
    public string? Details { get; set; }
    public string? Result { get; set; }
    public Guid? UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
