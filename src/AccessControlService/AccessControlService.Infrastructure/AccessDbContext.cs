using AccessControlService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessControlService.Infrastructure;

public class AccessDbContext : DbContext
{
    public AccessDbContext(DbContextOptions<AccessDbContext> options) : base(options) { }

    public DbSet<AccessRule> AccessRules => Set<AccessRule>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("access");

        modelBuilder.Entity<AccessRule>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.SubjectType).IsRequired();
            b.Property(x => x.SubjectId).IsRequired();
            b.Property(x => x.ResourceType).IsRequired();
            b.Property(x => x.Permission).IsRequired();

            b.HasIndex(x => new { x.ResourceType, x.ResourceId });
            b.HasIndex(x => new { x.SubjectType, x.SubjectId });
            b.HasIndex(x => x.ExpiresAt);

            // ВАЖНО: имя индекса фиксируем, как в БД
            b.HasIndex(x => new
            {
                x.SubjectType,
                x.SubjectId,
                x.ResourceType,
                x.ResourceId,
                x.Permission
            })
            .IsUnique()
            .HasDatabaseName("UX_AccessRules_Subject_Resource_Permission");
        });

        modelBuilder.Entity<AuditLog>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Action).IsRequired();
            b.Property(x => x.Details).IsRequired(false);
            b.HasIndex(x => x.CreatedAt);
        });
    }
}
