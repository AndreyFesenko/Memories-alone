using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface IEmailSender
{
    Task SendAsync(NotificationMessage message, CancellationToken ct = default);
}
