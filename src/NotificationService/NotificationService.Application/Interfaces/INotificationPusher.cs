// NotificationService.Application/Interfaces/INotificationPusher.cs
using NotificationService.Domain.Entities;
namespace NotificationService.Application.Interfaces;
public interface INotificationPusher
{
    Task NotifyUserAsync(string userId, NotificationMessage message);
    Task NotifyUsersAsync(IEnumerable<string> userIds, NotificationMessage message);
}
