using MediatR;
using AuditLoggingService.Application.DTOs;

namespace AuditLoggingService.Application.Queries;

public class SearchAuditLogsQuery : IRequest<PagedResult<AuditLogDto>>
{
    public string? Action { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 20;
}
