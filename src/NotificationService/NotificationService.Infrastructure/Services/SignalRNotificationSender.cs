// NotificationService.Infrastructure/Services/SignalRNotificationSender.cs
using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Hubs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Services;

public class SignalRNotificationSender : INotificationPusher
{
    private readonly IHubContext<NotificationHub> _hub;
    public SignalRNotificationSender(IHubContext<NotificationHub> hub) => _hub = hub;

    public Task NotifyUserAsync(string userId, NotificationMessage message) =>
        _hub.Clients.Group($"user:{userId}").SendAsync("notification", message);

    public async Task NotifyUsersAsync(IEnumerable<string> userIds, NotificationMessage message)
    {
        foreach (var uid in userIds) await NotifyUserAsync(uid, message);
    }
}
