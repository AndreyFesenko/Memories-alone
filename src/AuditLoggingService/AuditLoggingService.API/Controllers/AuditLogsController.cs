using AuditLoggingService.Application.Commands;
using AuditLoggingService.Application.DTOs;
using AuditLoggingService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuditLoggingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<AuditLogDto>> Create([FromBody] CreateAuditLogCommand cmd)
        => Ok(await _mediator.Send(cmd));

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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteAuditLogCommand { Id = id });
        return NoContent();
    }
}

public class DeleteAuditLogCommand : IRequest
{
    public Guid Id { get; set; }
}
