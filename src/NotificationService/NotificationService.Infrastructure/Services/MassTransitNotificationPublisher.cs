using MassTransit;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Services;

public class MassTransitNotificationPublisher : INotificationQueuePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitNotificationPublisher(IPublishEndpoint publishEndpoint)
        => _publishEndpoint = publishEndpoint;

    public async Task PublishAsync(NotificationMessage message, CancellationToken ct = default)
        => await _publishEndpoint.Publish(message, ct);
}
