using MediatR;
using AuditLoggingService.Application.DTOs;

namespace AuditLoggingService.Application.Commands;

public class CreateAuditLogCommand : IRequest<AuditLogDto>
{
    public string Action { get; set; } = default!;
    public string Details { get; set; } = default!;
    public Guid? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Result { get; set; }
}
