using NotificationService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces;

public interface IEmailSender
{
    Task SendAsync(NotificationMessage message, CancellationToken ct = default);
    Task SendAsync(NotificationMessage message, string template, object data, CancellationToken ct = default);
}
