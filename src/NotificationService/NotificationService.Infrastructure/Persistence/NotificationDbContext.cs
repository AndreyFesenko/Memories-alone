//C:\Users\user\Source\Repos\Memories-alone\src\NotificationService\NotificationService.Infrastructure\Persistence\NotificationDbContext.cs
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
        // 👇 Устанавливаем схему по умолчанию
        modelBuilder.HasDefaultSchema("notification");

        // 📨 Уведомления
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

        // 📋 Шаблоны уведомлений
        modelBuilder.Entity<NotificationTemplate>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Body).IsRequired();
            e.Property(x => x.Type).HasConversion<string>();
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
        });

        // 📜 Логирование действий (если включено в этот сервис)


        // Подключаем конфигурации (если появятся отдельные классы конфигурации)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
