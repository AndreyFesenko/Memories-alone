using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface INotificationQueuePublisher
{
    Task PublishAsync(NotificationMessage message, CancellationToken ct = default);
}
