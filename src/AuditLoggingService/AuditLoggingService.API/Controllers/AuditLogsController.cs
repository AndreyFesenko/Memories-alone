// src/AuditLoggingService/AuditLoggingService.API/Controllers/AuditLogsController.cs
using AuditLoggingService.Application.Commands;
using AuditLoggingService.Application.DTOs;
using AuditLoggingService.Application.Queries;
using AuditLoggingService.Application.Interfaces;
using AuditLoggingService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AuditLoggingService.Application.Commands;

namespace AuditLoggingService.API.Controllers;

[ApiController]
[Route("auditlogs")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuditLogRepository _repo;

    public AuditLogsController(IMediator mediator, IAuditLogRepository repo)
    {
        _mediator = mediator;
        _repo = repo;
    }

    /// <summary>Create audit log</summary>
    [HttpPost]
    public async Task<ActionResult<AuditLogDto>> Create([FromBody] CreateAuditLogCommand cmd)
        => Ok(await _mediator.Send(cmd));

    /// <summary>Search audit logs (paged)</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> Search(
        [FromQuery] string? action,
        [FromQuery] Guid? userId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 20)
    {
        var result = await _mediator.Send(new SearchAuditLogsQuery
        {
            Action = action,
            UserId = userId,
            From = from,
            To = to,
            Offset = offset,
            Limit = limit
        });
        return Ok(result);
    }

    /// <summary>Get audit log by id</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuditLogDto>> GetById(Guid id, CancellationToken ct)
    {
        var x = await _repo.GetByIdAsync(id, ct);
        if (x is null) return NotFound();

        var dto = new AuditLogDto
        {
            Id = x.Id,
            Action = x.Action,
            Target = x.Target,
            Details = x.Details,
            Result = x.Result,
            Data = x.Data,
            UserId = x.UserId,
            Timestamp = x.Timestamp,
            CreatedAt = x.CreatedAt,
            IpAddress = x.IpAddress,
            UserAgent = x.UserAgent
        };
        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteAuditLogCommand(id));
        return NoContent();
    }

}
