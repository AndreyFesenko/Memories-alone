using Microsoft.EntityFrameworkCore;
using AuditLoggingService.Domain.Entities;

namespace AuditLoggingService.Infrastructure.Persistence;

public class AuditLoggingDbContext : DbContext
{
    public AuditLoggingDbContext(DbContextOptions<AuditLoggingDbContext> options) : base(options) { }
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
}
