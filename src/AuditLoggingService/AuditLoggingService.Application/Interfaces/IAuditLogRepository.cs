using AuditLoggingService.Domain.Entities;
using AuditLoggingService.Application.DTOs;

namespace AuditLoggingService.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog log, CancellationToken ct);
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PagedResult<AuditLogDto>> SearchAsync(
        string? action, Guid? userId, DateTime? from, DateTime? to, int offset, int limit, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
