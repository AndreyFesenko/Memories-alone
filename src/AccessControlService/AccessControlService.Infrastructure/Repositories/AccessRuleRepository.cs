using AccessControlService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessControlService.Infrastructure.Repositories;

public class AccessRuleRepository
{
    private readonly AccessDbContext _db;

    public AccessRuleRepository(AccessDbContext db) => _db = db;

    public async Task<AccessRule?> GetAsync(Guid id) =>
        await _db.AccessRules.FindAsync(id);

    public async Task<List<AccessRule>> GetAllAsync() =>
        await _db.AccessRules.ToListAsync();

    public async Task<List<AccessRule>> GetForSubjectAsync(Guid subjectId) =>
        await _db.AccessRules.Where(x => x.SubjectId == subjectId).ToListAsync();

    public async Task<AccessRule> CreateAsync(AccessRule rule)
    {
        _db.AccessRules.Add(rule);
        await _db.SaveChangesAsync();
        return rule;
    }

    public async Task UpdateAsync(AccessRule rule)
    {
        _db.AccessRules.Update(rule);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var rule = await _db.AccessRules.FindAsync(id);
        if (rule != null)
        {
            _db.AccessRules.Remove(rule);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> CheckAccessAsync(Guid subjectId, Guid objectId, string accessType)
    {
        return await _db.AccessRules.AnyAsync(x =>
            x.SubjectId == subjectId && x.ObjectId == objectId && x.AccessType == accessType);
    }
}
