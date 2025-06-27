namespace NotificationService.Domain.Entities;

/// <summary>
/// Сущность шаблона уведомлений
/// </summary>
public class NotificationTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Body { get; set; } = default!;
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
