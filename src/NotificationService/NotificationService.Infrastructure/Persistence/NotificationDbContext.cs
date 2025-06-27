using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> opts)
        : base(opts) { }

    public DbSet<NotificationMessage> Notifications { get; set; }
    public DbSet<NotificationTemplate> Templates { get; set; }
}
