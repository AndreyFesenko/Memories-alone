// src/AccessControlService/AccessControlService.Infrastructure/Repositories/AccessRuleRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccessControlService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql; // для PostgresException и PostgresErrorCodes

namespace AccessControlService.Infrastructure.Repositories
{
    public class AccessRuleRepository
    {
        private readonly AccessDbContext _db;

        public AccessRuleRepository(AccessDbContext db) => _db = db;

        // Нормализация для единообразия (совпадает с уникальным индексом)
        private static void Normalize(AccessRule r)
        {
            r.SubjectType = (r.SubjectType ?? "User").Trim();
            r.SubjectId = (r.SubjectId ?? string.Empty).Trim();
            r.ResourceType = (r.ResourceType ?? "Memory").Trim();
            r.Permission = (r.Permission ?? "View").Trim();
        }

        /// <summary>
        /// Идемпотентное добавление правила.
        /// Если уникальный ключ уже есть — вернёт существующую запись.
        /// </summary>
        public async Task<AccessRule> CreateIfNotExistsAsync(AccessRule rule, CancellationToken ct = default)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            Normalize(rule);
            if (rule.Id == Guid.Empty) rule.Id = Guid.NewGuid();
            if (rule.CreatedAt == default) rule.CreatedAt = DateTime.UtcNow;

            // Быстрый путь: сначала попытаться найти, чтобы не ловить исключение без нужды
            var existing = await _db.AccessRules.AsNoTracking().FirstOrDefaultAsync(x =>
                x.SubjectType == rule.SubjectType &&
                x.SubjectId == rule.SubjectId &&
                x.ResourceType == rule.ResourceType &&
                x.ResourceId == rule.ResourceId &&
                x.Permission == rule.Permission, ct);

            if (existing is not null)
                return existing;

            _db.AccessRules.Add(rule);
            try
            {
                await _db.SaveChangesAsync(ct);
                return rule;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pex &&
                                               pex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                // Гонка: кто-то успел вставить между проверкой и SaveChanges
                existing = await _db.AccessRules.AsNoTracking().FirstOrDefaultAsync(x =>
                    x.SubjectType == rule.SubjectType &&
                    x.SubjectId == rule.SubjectId &&
                    x.ResourceType == rule.ResourceType &&
                    x.ResourceId == rule.ResourceId &&
                    x.Permission == rule.Permission, ct);

                if (existing is not null) return existing;
                throw; // если почему-то не нашли — пробрасываем
            }
        }

        public async Task UpdateAsync(AccessRule rule, CancellationToken ct = default)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            Normalize(rule);

            _db.AccessRules.Update(rule);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _db.AccessRules.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity != null)
            {
                _db.AccessRules.Remove(entity);
                await _db.SaveChangesAsync(ct);
            }
        }

        public Task<AccessRule?> GetAsync(Guid id, CancellationToken ct = default) =>
            _db.AccessRules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<List<AccessRule>> GetAllAsync(CancellationToken ct = default) =>
            _db.AccessRules.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(ct);

        /// <summary>
        /// Проверка доступа по ACL.
        /// </summary>
        public Task<bool> CheckAccessAsync(
            string subjectId,
            Guid resourceId,
            string permission,
            string? subjectType = null,
            string resourceType = "Memory",
            CancellationToken ct = default)
        {
            subjectId = (subjectId ?? string.Empty).Trim();
            permission = (permission ?? "View").Trim();
            subjectType = subjectType?.Trim();
            resourceType = (resourceType ?? "Memory").Trim();

            var now = DateTime.UtcNow;

            return _db.AccessRules.AsNoTracking().AnyAsync(x =>
                x.SubjectId == subjectId &&
                (subjectType == null || x.SubjectType == subjectType) &&
                x.ResourceType == resourceType &&
                x.ResourceId == resourceId &&
                x.Permission == permission &&
                (x.ExpiresAt == null || x.ExpiresAt > now), ct);
        }

        public Task<List<AccessRule>> GetByResourceAsync(Guid resourceId, CancellationToken ct = default) =>
            _db.AccessRules.AsNoTracking()
               .Where(r => r.ResourceId == resourceId)
               .OrderByDescending(r => r.CreatedAt)
               .ToListAsync(ct);

        /// <summary>
        /// Список шарингов по Memory для UI/админки.
        /// </summary>
        public Task<List<AccessRule>> GetSharesForMemoryAsync(Guid memoryId, CancellationToken ct = default) =>
            _db.AccessRules.AsNoTracking()
               .Where(x => x.ResourceType == "Memory" && x.ResourceId == memoryId)
               .OrderByDescending(x => x.CreatedAt)
               .ToListAsync(ct);

        /// <summary>
        /// Удаление правила по составному ключу (для /revoke).
        /// </summary>
        public Task<int> DeleteByCompositeAsync(
            string subjectId,
            string subjectType,
            string resourceType,
            Guid resourceId,
            string permission,
            CancellationToken ct = default)
        {
            subjectId = (subjectId ?? string.Empty).Trim();
            subjectType = (subjectType ?? "User").Trim();
            resourceType = (resourceType ?? "Memory").Trim();
            permission = (permission ?? "View").Trim();

            return _db.AccessRules
                .Where(x => x.SubjectId == subjectId &&
                            x.SubjectType == subjectType &&
                            x.ResourceType == resourceType &&
                            x.ResourceId == resourceId &&
                            x.Permission == permission)
                .ExecuteDeleteAsync(ct);
        }
    }
}
