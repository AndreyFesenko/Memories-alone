namespace NotificationService.Application.Interfaces;

public interface IAuditService
{
    Task WriteAuditAsync(string action, string userId, string details, CancellationToken ct = default);
}
    