namespace NotificationService.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = default!;
    public string Action { get; set; } = default!;   // send, receive, fail, delivered, opened, etc.
    public string Target { get; set; } = default!;   // например, notificationId
    public string Data { get; set; } = default!;     // JSON с деталями (optional)
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
