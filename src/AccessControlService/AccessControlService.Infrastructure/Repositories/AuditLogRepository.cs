using AccessControlService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessControlService.Infrastructure.Repositories;

public class AuditLogRepository
{
    private readonly AccessDbContext _db;

    public AuditLogRepository(AccessDbContext db) => _db = db;

    public async Task AddAsync(AuditLog log)
    {
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetAllAsync() =>
        await _db.AuditLogs.OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync();
}
