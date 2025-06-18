using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityService.Domain.Entities;
using IdentityService.Application.Interfaces;
using IdentityService.Infrastructure.Persistence;

namespace IdentityService.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly MemoriesDbContext _db;
        public AuditService(MemoriesDbContext db) => _db = db;

        public async Task LogAsync(string action, string details, Guid? userId = null, CancellationToken ct = default)
        {
            _db.AuditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                Details = details,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(ct);
        }
    }
}
