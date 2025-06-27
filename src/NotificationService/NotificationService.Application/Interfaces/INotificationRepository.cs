using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

/// <summary>
/// Репозиторий для сообщений-уведомлений
/// </summary>
public interface INotificationRepository
{
    Task<NotificationMessage?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<NotificationMessage>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(NotificationMessage notification, CancellationToken ct = default);
    Task UpdateAsync(NotificationMessage notification, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
