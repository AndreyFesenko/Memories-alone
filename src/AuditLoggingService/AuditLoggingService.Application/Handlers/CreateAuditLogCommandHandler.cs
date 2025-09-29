//C:\Users\user\source\repos\Memories-alone\src\AuditLoggingService\AuditLoggingService.Application\Handlers\CreateAuditLogCommandHandler.cs
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
            Target = request.Target,
            Details = request.Details,
            Result = request.Result,
            Data = request.Data,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UserId = request.UserId
        };

        await _repo.CreateAsync(log, ct);

        return new AuditLogDto
        {
            Id = log.Id,
            Action = log.Action,
            Target = log.Target,
            Details = log.Details,
            Result = log.Result,
            Data = log.Data,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            Timestamp = log.Timestamp,
            CreatedAt = log.CreatedAt,
            UserId = log.UserId
        };
    }
}
