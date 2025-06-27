using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Consumers;

public class NotificationConsumer : IConsumer<NotificationMessage>
{
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(ILogger<NotificationConsumer> logger)
        => _logger = logger;

    public async Task Consume(ConsumeContext<NotificationMessage> context)
    {
        var msg = context.Message;
        // Здесь логика отправки email/sms/push и аудит
        _logger.LogInformation("Notification consumed: {Id} {Title}", msg.Id, msg.Title);
        // TODO: Отправить уведомление
        await Task.CompletedTask;
    }
}
