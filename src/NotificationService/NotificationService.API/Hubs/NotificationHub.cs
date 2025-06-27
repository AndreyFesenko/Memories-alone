// NotificationService.API/Hubs/NotificationHub.cs
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.API.Hubs;

public class NotificationHub : Hub
{
    // Пример подписки на сообщения: клиент вызывает hubConnection.invoke("Join", userId)
    public Task Join(string userId)
        => Groups.AddToGroupAsync(Context.ConnectionId, userId);

    public Task Leave(string userId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
}
