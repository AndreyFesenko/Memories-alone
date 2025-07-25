using Microsoft.EntityFrameworkCore;
using AuditLoggingService.Application.DTOs;
using AuditLoggingService.Application.Interfaces;
using AuditLoggingService.Domain.Entities;
using AuditLoggingService.Infrastructure.Persistence;

namespace AuditLoggingService.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AuditLoggingDbContext _db;

    public AuditLogRepository(AuditLoggingDbContext db) => _db = db;

    public async Task<AuditLog> CreateAsync(AuditLog log, CancellationToken ct)
    {
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync(ct);
        return log;
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.AuditLogs.FindAsync(new object[] { id }, ct);

    public async Task<PagedResult<AuditLogDto>> SearchAsync(
        string? action, Guid? userId, DateTime? from, DateTime? to, int offset, int limit, CancellationToken ct)
    {
        var query = _db.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(action))
            query = query.Where(x => x.Action == action);

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value.ToString());

        if (from.HasValue)
            query = query.Where(x => x.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.CreatedAt <= to.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                Action = x.Action,
                Details = x.Details,
                UserId = x.UserId,
                CreatedAt = x.CreatedAt,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                Result = x.Result
            })
            .ToListAsync(ct);

        return new PagedResult<AuditLogDto> { TotalCount = total, Items = items };
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var log = await _db.AuditLogs.FindAsync(new object[] { id }, ct);
        if (log != null)
        {
            _db.AuditLogs.Remove(log);
            await _db.SaveChangesAsync(ct);
        }
    }
}
