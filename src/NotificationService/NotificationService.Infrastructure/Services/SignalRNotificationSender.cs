using Microsoft.AspNetCore.SignalR;
using NotificationService.Domain.Entities;
using NotificationService.Application.Hubs;

namespace NotificationService.Infrastructure.Services;

public class SignalRNotificationSender
{
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRNotificationSender(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public async Task NotifyUserAsync(string userId, NotificationMessage message)
    {
        await _hub.Clients.Group($"user:{userId}").SendAsync("notification", message);
    }

    public async Task NotifyUsersAsync(IEnumerable<string> userIds, NotificationMessage message)
    {
        foreach (var userId in userIds)
        {
            await NotifyUserAsync(userId, message);
        }
    }
}
