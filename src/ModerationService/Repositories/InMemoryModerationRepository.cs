using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModerationService.Models;

namespace ModerationService.Repositories
{
    public class InMemoryModerationRepository : IModerationRepository
    {
        private readonly ConcurrentDictionary<Guid, ModerationReview> _store = new();

        public Task<ModerationReview> CreateAsync(ModerationReview review)
        {
            _store[review.Id] = review;
            return Task.FromResult(review);
        }

        public Task<ModerationReview?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var v);
            return Task.FromResult(v);
        }

        public Task<IEnumerable<ModerationReview>> ListAsync(string? status = null, int page = 1, int pageSize = 20)
        {
            var q = _store.Values.AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<ReviewStatus>(status, true, out var st))
                {
                    q = q.Where(x => x.Status == st);
                }
                else
                {
                    q = Enumerable.Empty<ModerationReview>().AsQueryable();
                }
            }

            var items = q
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult<IEnumerable<ModerationReview>>(items);
        }

        public Task<bool> UpdateAsync(ModerationReview review)
        {
            if (!_store.ContainsKey(review.Id)) return Task.FromResult(false);
            _store[review.Id] = review;
            return Task.FromResult(true);
        }
    }
}
