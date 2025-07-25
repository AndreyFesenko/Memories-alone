using Microsoft.EntityFrameworkCore;
using AuditLoggingService.Domain.Entities;
using AuditLoggingService.Infrastructure.Persistence;

namespace AuditLoggingService.Infrastructure.Persistence;

public class AuditLoggingDbContext : DbContext
{
    public AuditLoggingDbContext(DbContextOptions<AuditLoggingDbContext> options) : base(options) { }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("audit");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditLoggingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
