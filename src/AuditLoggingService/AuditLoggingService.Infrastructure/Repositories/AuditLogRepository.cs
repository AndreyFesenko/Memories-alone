using Microsoft.EntityFrameworkCore;
using AuditLoggingService.Application.DTOs;
using AuditLoggingService.Application.Interfaces;
using AuditLoggingService.Domain.Entities;
using AuditLoggingService.Infrastructure.Persistence;

namespace AuditLoggingService.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private const int DEFAULT_LIMIT = 20;
    private const int MAX_LIMIT = 200;

    private readonly AuditLoggingDbContext _db;

    public AuditLogRepository(AuditLoggingDbContext db) => _db = db;

    public async Task<AuditLog> CreateAsync(AuditLog log, CancellationToken ct)
    {
        await _db.AuditLogs.AddAsync(log, ct);
        await _db.SaveChangesAsync(ct);
        return log;
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.AuditLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<AuditLogDto>> SearchAsync(
        string? action, Guid? userId, DateTime? from, DateTime? to, int offset, int limit, CancellationToken ct)
    {
        if (offset < 0) offset = 0;
        if (limit <= 0) limit = DEFAULT_LIMIT;
        if (limit > MAX_LIMIT) limit = MAX_LIMIT;

        IQueryable<AuditLog> query = _db.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(x => x.Action == action);

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId);

        if (from.HasValue)
            query = query.Where(x => x.Timestamp >= from.Value);
        if (to.HasValue)
            query = query.Where(x => x.Timestamp <= to.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip(offset)
            .Take(limit)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                Action = x.Action,
                Target = x.Target,
                Details = x.Details,
                Result = x.Result,
                Data = x.Data,
                UserId = x.UserId,
                Timestamp = x.Timestamp,
                CreatedAt = x.CreatedAt,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent
            })
            .ToListAsync(ct);

        return new PagedResult<AuditLogDto> { TotalCount = total, Items = items };
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var log = await _db.AuditLogs.FindAsync(new object[] { id }, ct);
        if (log == null) return;

        _db.AuditLogs.Remove(log);
        await _db.SaveChangesAsync(ct);
    }
}
