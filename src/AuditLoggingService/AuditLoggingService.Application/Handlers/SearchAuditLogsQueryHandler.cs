using AuditLoggingService.Application.DTOs;
using AuditLoggingService.Application.Interfaces;
using AuditLoggingService.Application.Queries;
using MediatR;

namespace AuditLoggingService.Application.Handlers;

public class SearchAuditLogsQueryHandler : IRequestHandler<SearchAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _repo;

    public SearchAuditLogsQueryHandler(IAuditLogRepository repo) => _repo = repo;

    public async Task<PagedResult<AuditLogDto>> Handle(SearchAuditLogsQuery query, CancellationToken ct)
    {
        var result = await _repo.SearchAsync(
            action: query.Action,
            userId: query.UserId,
            from: query.From,
            to: query.To,
            offset: query.Offset,
            limit: query.Limit,
            ct: ct);

        return result;
    }
}
