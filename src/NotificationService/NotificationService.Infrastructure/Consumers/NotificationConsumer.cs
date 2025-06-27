using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Services;

public class NotificationConsumer : IConsumer<NotificationMessage>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(IEmailSender emailSender, ILogger<NotificationConsumer> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotificationMessage> context)
    {
        try
        {
            await _emailSender.SendAsync(context.Message);
            _logger.LogInformation("Notification delivered (UserId: {UserId})", context.Message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Notification send failed (UserId: {UserId})", context.Message.UserId);
            throw; // MassTransit автоматически закинет в retry/dlq
        }
    }
}
