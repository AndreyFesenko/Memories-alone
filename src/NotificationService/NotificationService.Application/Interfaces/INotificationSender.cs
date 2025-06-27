using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

/// <summary>
/// Абстракция для отправки уведомлений (реализации: Email, SMS, Push)
/// </summary>
public interface INotificationSender
{
    Task SendAsync(NotificationMessage notification, CancellationToken ct = default);
}
