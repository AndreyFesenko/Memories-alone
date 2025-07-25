namespace NotificationService.Application.Messages;

public class AuditLogMessage
{
    public string UserId { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string Target { get; set; } = default!;
    public string? Data { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}
