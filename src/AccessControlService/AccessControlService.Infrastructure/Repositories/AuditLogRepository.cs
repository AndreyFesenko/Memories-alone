using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccessControlService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessControlService.Infrastructure.Repositories
{
    public class AuditLogRepository
    {
        private readonly AccessDbContext _db;

        public AuditLogRepository(AccessDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(AuditLog log, CancellationToken ct = default)
        {
            if (log.Id == Guid.Empty) log.Id = Guid.NewGuid();
            if (log.CreatedAt == default) log.CreatedAt = DateTime.UtcNow;

            _db.AuditLogs.Add(log);
            await _db.SaveChangesAsync(ct);
        }

        public Task<List<AuditLog>> GetAllAsync(CancellationToken ct = default) =>
            _db.AuditLogs.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);
    }
}
