// src/NotificationService/NotificationService.Infrastructure/Services/AuditService.cs
using MassTransit;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Messages;
using System.Text.Json;

namespace NotificationService.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IPublishEndpoint _publish;

    public AuditService(IPublishEndpoint publish)
        => _publish = publish;

    public async Task LogAsync(
        string userId,
        string action,
        string target,
        object? data = null,
        string? ip = null,
        string? ua = null,
        CancellationToken ct = default)
    {
        var message = new AuditLogMessage
        {
            UserId = userId,
            Action = action,
            Target = target,
            Data = data != null ? JsonSerializer.Serialize(data) : null,
            IpAddress = ip,
            UserAgent = ua,
            Timestamp = DateTime.UtcNow
        };

        await _publish.Publish(message, ct);
    }
}
