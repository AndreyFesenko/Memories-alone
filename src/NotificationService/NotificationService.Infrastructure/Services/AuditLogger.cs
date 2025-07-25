using NotificationService.Application.Interfaces;
using Microsoft.Extensions.Logging;
//using Shared.Messaging.Messages;

public class AuditLogger : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(ILogger<AuditLogger> logger) => _logger = logger;

    public Task LogAsync(string action, string details, string? userId, CancellationToken ct)
    {
        _logger.LogInformation("AUDIT: {Action} - {Details} (User: {UserId})", action, details, userId);
        return Task.CompletedTask;
    }
}