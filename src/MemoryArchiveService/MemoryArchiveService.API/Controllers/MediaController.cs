using Microsoft.AspNetCore.Mvc;
using MediatR;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Queries;

namespace MemoryArchiveService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;
    public MediaController(IMediator mediator) => _mediator = mediator;

    [HttpPost("upload")]
    public async Task<ActionResult<string>> Upload([FromForm] UploadMediaCommand cmd)
        => Ok(await _mediator.Send(cmd));

    [HttpGet("{id}")]
    public async Task<ActionResult<MediaFileDto>> Get(Guid id)
    {
        var dto = await _mediator.Send(new GetMediaQuery { Id = id });
        return dto is null ? NotFound() : Ok(dto);
    }
}
