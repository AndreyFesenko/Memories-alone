using AuditLoggingService.Application.Interfaces;
using MediatR;

namespace AuditLoggingService.API.Controllers;

public class DeleteAuditLogCommandHandler : IRequestHandler<DeleteAuditLogCommand>
{
    private readonly IAuditLogRepository _repo;
    public DeleteAuditLogCommandHandler(IAuditLogRepository repo) => _repo = repo;
    public async Task Handle(DeleteAuditLogCommand request, CancellationToken ct)
        => await _repo.DeleteAsync(request.Id, ct);
}
