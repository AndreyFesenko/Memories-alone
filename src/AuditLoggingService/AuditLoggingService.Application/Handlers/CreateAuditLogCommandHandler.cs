using MediatR;
using AuditLoggingService.Application.Commands;
using AuditLoggingService.Application.DTOs;
using AuditLoggingService.Application.Interfaces;
using AuditLoggingService.Domain.Entities;

namespace AuditLoggingService.Application.Handlers;

public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, AuditLogDto>
{
    private readonly IAuditLogRepository _repo;

    public CreateAuditLogCommandHandler(IAuditLogRepository repo) => _repo = repo;

    public async Task<AuditLogDto> Handle(CreateAuditLogCommand request, CancellationToken ct)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = request.Action,
            Details = request.Details,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            Result = request.Result
        };
        await _repo.CreateAsync(log, ct);
        return new AuditLogDto
        {
            Id = log.Id,
            Action = log.Action,
            Details = log.Details,
            UserId = log.UserId,
            CreatedAt = log.CreatedAt,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            Result = log.Result
        };
    }
}
