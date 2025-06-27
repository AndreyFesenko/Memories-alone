namespace NotificationService.Domain.Entities;

public class NotificationMessage
{

    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string Channel { get; set; } = "Email"; // Email, Sms, Push, Webhook, etc.
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    public string? FailureReason { get; set; }
    public string Subject { get; set; } = default!;
    public string Recipient { get; set; } = default!;
    public string Template { get; set; } = default!;

    public string TemplateId { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool IsRead { get; set; }
    public NotificationType Type { get; set; }

}

//public enum NotificationType
//{
//    Info,
//    Warning,
//    Error,
//    Promotion
//}

//public enum NotificationChannel
//{
//    Email,
//    Sms,
//    Push,
//    Webhook,
//    SignalR
//}

public enum NotificationStatus
{
    Queued,
    Sent,
    Failed
}
