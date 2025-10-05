using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModerationService.Models;

namespace ModerationService.Repositories
{
    public interface IModerationRepository
    {
        Task<ModerationReview> CreateAsync(ModerationReview review);
        Task<ModerationReview?> GetByIdAsync(Guid id);
        Task<IEnumerable<ModerationReview>> ListAsync(string? status = null, int page = 1, int pageSize = 20);
        Task<bool> UpdateAsync(ModerationReview review);
    }
}
