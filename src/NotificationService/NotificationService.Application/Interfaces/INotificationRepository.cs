using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface INotificationRepository
{
    Task<NotificationMessage?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<NotificationMessage>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(NotificationMessage msg, CancellationToken ct = default);
    // ...Update, Delete
}
