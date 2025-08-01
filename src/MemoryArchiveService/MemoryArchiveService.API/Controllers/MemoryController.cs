//C:\Users\user\Source\Repos\Memories-alone\src\MemoryArchiveService\MemoryArchiveService.API\Controllers\MemoryController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.API.Models;
using MemoryArchiveService.API.Mapping;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Queries;

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

    /// <summary>
    /// Создание нового воспоминания с файлом
    /// </summary>
    [HttpPost]
    [DisableRequestSizeLimit]
    [ProducesResponseType(typeof(MemoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] CreateMemoryForm form, CancellationToken ct)
    {
        if (form.File == null || form.File.Length == 0)
            return BadRequest("Файл обязателен");

        var command = await form.MapToCommandAsync(ct);
        var result = await _mediator.Send(command, ct);

        return Ok(result);
    }

    /// <summary>
    /// Получить воспоминание по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MemoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetMemoryByIdQuery { Id = id };
        var result = await _mediator.Send(query, ct);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Получить воспоминания пользователя с фильтром и пагинацией
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(PagedResult<MemoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser(
        Guid userId,
        [FromQuery] string? accessLevel,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetMemoriesByUserQuery
        {
            UserId = userId,
            AccessLevel = accessLevel,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }
}
