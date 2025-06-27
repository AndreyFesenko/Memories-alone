using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _db;

    public NotificationRepository(NotificationDbContext db)
    {
        _db = db;
    }

    public async Task<NotificationMessage?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Notifications.FindAsync(new object[] { id }, ct);

    public async Task<List<NotificationMessage>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken ct = default)
        => await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(NotificationMessage msg, CancellationToken ct = default)
    {
        await _db.Notifications.AddAsync(msg, ct);
        await _db.SaveChangesAsync(ct);
    }
}
