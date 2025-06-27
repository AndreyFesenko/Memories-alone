using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly NotificationDbContext _db;

    public NotificationTemplateRepository(NotificationDbContext db)
    {
        _db = db;
    }

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Templates.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<NotificationTemplate?> GetByNameAsync(string name, string? type = null, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(type))
            return await _db.Templates.FirstOrDefaultAsync(x => x.Name == name, ct);

        if (Enum.TryParse<NotificationType>(type, ignoreCase: true, out var enumType))
        {
            return await _db.Templates.FirstOrDefaultAsync(
                x => x.Name == name && x.Type == enumType, ct);
        }

        return null;
    }

    public async Task<List<NotificationTemplate>> GetAllAsync(CancellationToken ct = default)
        => await _db.Templates.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);

    public async Task AddAsync(NotificationTemplate template, CancellationToken ct = default)
    {
        await _db.Templates.AddAsync(template, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(NotificationTemplate template, CancellationToken ct = default)
    {
        _db.Templates.Update(template);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var t = await _db.Templates.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (t != null)
        {
            _db.Templates.Remove(t);
            await _db.SaveChangesAsync(ct);
        }
    }
}
