using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken ct = default);
    Task<List<AuditLog>> GetByUserAsync(string userId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<List<AuditLog>> SearchAsync(string? action = null, string? target = null, int page = 1, int pageSize = 20, CancellationToken ct = default);
}
