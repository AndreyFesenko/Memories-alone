// src/NotificationService/NotificationService.Domain/Entities/NotificationMessage.cs
namespace NotificationService.Domain.Entities;

public class NotificationMessage
{
    public Guid Id { get; set; }
    public string? UserId { get; set; } = default!;

    // Тема (subject) уведомления
    public string Subject { get; set; } = default!;

    // Основной текст (body/message) уведомления
    public string Message { get; set; } = default!;

    // Альтернативные поля — если нужно
    public string? Title { get; set; }    // Для push/webhook
    public bool IsRead { get; set; }
    public string Type { get; set; } = default!;
    public string Channel { get; set; } = "Email"; // Email, Sms, Push, Webhook, etc.
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Status { get; set; } // Sent, Failed, Queued, etc.
    public string? FailureReason { get; set; }
}
