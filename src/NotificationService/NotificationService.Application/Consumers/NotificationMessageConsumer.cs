// src/NotificationService/NotificationService.Application/Consumers/NotificationMessageConsumer.cs
using MassTransit;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Consumers;

public class NotificationMessageConsumer : IConsumer<NotificationMessage>
{
    public async Task Consume(ConsumeContext<NotificationMessage> context)
    {
        var message = context.Message;
        // Твоя логика обработки уведомления:
        Console.WriteLine($"[Consumer] Got notification for: {message.Recipient}, subject: {message.Subject}");
        await Task.CompletedTask;
    }
}
