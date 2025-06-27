using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public DbSet<NotificationMessage> Notifications { get; set; }
    public DbSet<NotificationTemplate> Templates { get; set; }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Recipient).IsRequired();
            e.Property(x => x.Subject).IsRequired();
            e.Property(x => x.Body).IsRequired();
            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.Channel).IsRequired();
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<NotificationTemplate>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Body).IsRequired();
            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
