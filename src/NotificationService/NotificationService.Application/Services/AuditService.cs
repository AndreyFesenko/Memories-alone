// src/NotificationService/NotificationService.Infrastructure/Services/AuditService.cs
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _repo;

    public AuditService(IAuditLogRepository repo)
        => _repo = repo;

    public async Task LogAsync(
        string userId,
        string action,
        string target,
        object? data = null,
        string? ip = null,
        string? ua = null,
        CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            Target = target,
            Data = data != null ? System.Text.Json.JsonSerializer.Serialize(data) : null,
            IpAddress = ip,
            UserAgent = ua
        };
        await _repo.AddAsync(log, ct);
    }
}
