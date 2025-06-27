namespace NotificationService.Application.DTOs;

/// <summary>
/// DTO для передачи данных уведомления (response/request)
/// </summary>
public class NotificationDto
{

    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string Channel { get; set; } = "Email";
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string Status { get; set; } = "Queued";
    public string? FailureReason { get; set; }


    /// <summary>
    /// Email, телефон, userId или токен, куда отправлять
    /// </summary>
    public string Recipient { get; set; } = default!;

    /// <summary>
    /// Тип уведомления (Email, SMS, Push, Webhook, InApp)
    /// </summary>
    public string Type { get; set; } = default!;

    /// <summary>
    /// Заголовок или тема уведомления
    /// </summary>
    public string Subject { get; set; } = default!;

    /// <summary>
    /// Признак успешной доставки
    /// </summary>
    public bool Delivered { get; set; }

    /// <summary>
    /// Ошибка доставки, если была
    /// </summary>
    public string? Error { get; set; }


}
