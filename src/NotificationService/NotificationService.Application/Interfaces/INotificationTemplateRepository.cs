// src/NotificationService/NotificationService.Application/Interfaces/INotificationTemplateRepository.cs
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface INotificationTemplateRepository
{
    Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<NotificationTemplate?> GetByNameAsync(string name, string? type = null, CancellationToken ct = default);
    Task<List<NotificationTemplate>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(NotificationTemplate template, CancellationToken ct = default);   // <--- добавить
    Task UpdateAsync(NotificationTemplate template, CancellationToken ct = default); // <--- если нужно
    Task DeleteAsync(Guid id, CancellationToken ct = default); // <--- если нужно
}
