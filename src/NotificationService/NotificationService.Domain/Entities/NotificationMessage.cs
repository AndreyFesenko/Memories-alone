//  src\NotificationService\NotificationService.Domain\Entities\NotificationMessage.cs
namespace NotificationService.Domain.Entities;

/// <summary>
/// Сущность для хранения сообщения уведомления.
/// </summary>
public class NotificationMessage
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Recipient { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Body { get; set; } = default!;
    public NotificationType Type { get; set; }
    public string Channel { get; set; } = "Email"; // Email, Sms, Push, Webhook, etc.
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public NotificationStatus Status { get; set; }
    public string? FailureReason { get; set; }
}

public enum NotificationType
{
    General,
    Registration,
    PasswordReset,
    SystemAlert
}

public enum NotificationStatus
{
    Queued,
    Sent,
    Failed
}
