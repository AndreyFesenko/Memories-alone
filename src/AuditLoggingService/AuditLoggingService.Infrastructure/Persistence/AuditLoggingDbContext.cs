using Microsoft.EntityFrameworkCore;
using AuditLoggingService.Domain.Entities;

namespace AuditLoggingService.Infrastructure.Persistence;

public class AuditLoggingDbContext : DbContext
{
    public AuditLoggingDbContext(DbContextOptions<AuditLoggingDbContext> options) : base(options) { }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("audit");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditLoggingDbContext).Assembly);

        // На случай отсутствия конфигураций через IEntityTypeConfiguration:
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.ToTable("AuditLogs");
            e.HasKey(x => x.Id);

            e.Property(x => x.Action).IsRequired().HasMaxLength(100);
            e.Property(x => x.Target).IsRequired().HasMaxLength(100);
            e.Property(x => x.IpAddress).HasMaxLength(45);
            e.Property(x => x.UserAgent).HasMaxLength(256);

            e.HasIndex(x => x.Timestamp);
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => x.Action);
            e.HasIndex(x => x.UserId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
