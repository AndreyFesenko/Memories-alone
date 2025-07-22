using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly NotificationDbContext _db;
    public AuditLogRepository(NotificationDbContext db) => _db = db;

    public async Task AddAsync(AuditLog log, CancellationToken ct = default)
    {
        await _db.AuditLogs.AddAsync(log, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<AuditLog>> GetByUserAsync(string userId, int page = 1, int pageSize = 20, CancellationToken ct = default)
        => await _db.AuditLogs
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<List<AuditLog>> SearchAsync(string? action = null, string? target = null, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var q = _db.AuditLogs.AsQueryable();
        if (!string.IsNullOrEmpty(action))
            q = q.Where(x => x.Action == action);
        if (!string.IsNullOrEmpty(target))
            q = q.Where(x => x.Target == target);

        return await q.OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }
}
