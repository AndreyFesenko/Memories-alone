using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string action, string details, Guid? userId = null, CancellationToken ct = default);
    }
}
