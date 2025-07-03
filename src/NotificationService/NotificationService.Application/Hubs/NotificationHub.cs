// NotificationService.Application/Hubs/NotificationHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace NotificationService.Application.Hubs;

public class NotificationHub : Hub
{
    // Можно вызывать с клиента: connection.invoke("Subscribe", "user-id");
    public async Task Subscribe(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
    }

    public async Task Unsubscribe(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
    }

    // Пример: Получение своего userId из Claims (если нужна авторизация)
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");

        await base.OnDisconnectedAsync(exception);
    }
}
