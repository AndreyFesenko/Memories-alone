using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Commands;
using NotificationService.Application.Queries;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationTemplateController : ControllerBase
{
    private readonly IMediator _mediator;
    public NotificationTemplateController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<NotificationTemplateDto>>> GetAll()
        => Ok(await _mediator.Send(new GetAllTemplatesQuery()));

    [HttpPost]
    public async Task<ActionResult<NotificationTemplateDto>> Create([FromBody] CreateTemplateCommand cmd)
        => Ok(await _mediator.Send(cmd));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<NotificationTemplateDto>> Update(Guid id, [FromBody] UpdateTemplateCommand cmd)
    {
        cmd.Id = id;
        return Ok(await _mediator.Send(cmd));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTemplateCommand { Id = id });
        return Ok();
    }
}
