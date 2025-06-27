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
        => await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<NotificationMessage>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken ct = default)
        => await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(NotificationMessage notification, CancellationToken ct = default)
    {
        await _db.Notifications.AddAsync(notification, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(NotificationMessage notification, CancellationToken ct = default)
    {
        _db.Notifications.Update(notification);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (n != null)
        {
            _db.Notifications.Remove(n);
            await _db.SaveChangesAsync(ct);
        }
    }
}
