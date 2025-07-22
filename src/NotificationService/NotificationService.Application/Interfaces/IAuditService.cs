// src/NotificationService/NotificationService.Application/Interfaces/IAuditService.cs
namespace NotificationService.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string userId,
        string action,
        string target,
        object? data = null,
        string? ip = null,
        string? ua = null,
        CancellationToken ct = default);
}
