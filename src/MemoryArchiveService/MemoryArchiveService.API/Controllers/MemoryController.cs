using MediatR;
using Microsoft.AspNetCore.Mvc;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.API.Models;

namespace MemoryArchiveService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MemoryController> _logger;

    public MemoryController(IMediator mediator, ILogger<MemoryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Create([FromForm] CreateMemoryForm form)
    {
        if (form.File == null || form.File.Length == 0)
            return BadRequest("Файл обязателен");

        using var stream = form.File.OpenReadStream();

        var command = new CreateMemoryCommand
        {
            OwnerId = form.OwnerId,
            Title = form.Title,
            Description = form.Description,
            AccessLevel = form.AccessLevel ?? "Private",
            Tags = form.Tags?.ToList(),

            FileName = form.File.FileName,
            ContentType = form.File.ContentType,
            MediaType = form.MediaType,
            FileStream = stream
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
