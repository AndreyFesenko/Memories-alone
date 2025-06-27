// src/NotificationService/NotificationService.Domain/Entities/NotificationTemplate.cs
namespace NotificationService.Domain.Entities;

public class NotificationTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string BodyTemplate { get; set; } = default!;
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}
