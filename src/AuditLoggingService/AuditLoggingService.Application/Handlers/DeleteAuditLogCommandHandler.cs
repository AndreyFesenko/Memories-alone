using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AuditLoggingService.Application.Commands;
using AuditLoggingService.Application.Interfaces;

namespace AuditLoggingService.Application.Handlers;

public sealed class DeleteAuditLogCommandHandler : IRequestHandler<DeleteAuditLogCommand>
{
    private readonly IAuditLogRepository _repo;

    public DeleteAuditLogCommandHandler(IAuditLogRepository repo) => _repo = repo;

    public async Task Handle(DeleteAuditLogCommand request, CancellationToken ct)
    {
        await _repo.DeleteAsync(request.Id, ct);
    }
}
