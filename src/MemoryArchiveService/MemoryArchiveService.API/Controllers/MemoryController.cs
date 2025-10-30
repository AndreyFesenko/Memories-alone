// src/MemoryArchiveService/MemoryArchiveService.API/Controllers/MemoryController.cs
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MemoryArchiveService.API.Mapping;
using MemoryArchiveService.API.Models;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Queries;
using MemoryArchiveService.Infrastructure.Services;

namespace MemoryArchiveService.API.Controllers;

[ApiController]
[Route("api/memory")]
[Produces("application/json")]
public class MemoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MemoryController> _logger;
    private readonly IPublicUrlResolver _urlResolver;

    public MemoryController(
        IMediator mediator,
        ILogger<MemoryController> logger,
        IPublicUrlResolver urlResolver)
    {
        _mediator = mediator;
        _logger = logger;
        _urlResolver = urlResolver;
    }

    /// <summary>
    /// Создание нового воспоминания с файлами (мультизагрузка)
    /// </summary>
    [HttpPost]
    [DisableRequestSizeLimit]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(MemoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] CreateMemoryForm form, CancellationToken ct)
    {
        // мультизагрузка: ожидаем повторяющееся поле File => form.Files
        if (form.Files is null || form.Files.Count == 0)
            return BadRequest("Не переданы файлы. Ожидается повторяющееся multipart-поле 'File'.");

        var command = await form.MapToCommandAsync(ct);
        var result = await _mediator.Send(command, ct);

        NormalizeMediaUrls(result);

        // 201 Created c Location на GetById
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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

        NormalizeMediaUrls(result);
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

        // Нормализуем публичные ссылки во всех элементах страницы
        if (result?.Items != null)
        {
            foreach (var dto in result.Items)
                NormalizeMediaUrls(dto);
        }

        return Ok(result);
    }

    /// <summary>
    /// Удалить воспоминание
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteMemoryCommand
        {
            Id = id,
            RequesterId = TryGetUserId()
        }, ct);

        return ok ? NoContent() : NotFound();
    }

    private Guid? TryGetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(sub, out var id) ? id : (Guid?)null;
    }

    private void NormalizeMediaUrls(MemoryDto? dto)
    {
        if (dto?.MediaFiles == null) return;

        foreach (var m in dto.MediaFiles)
        {
            // Отдаём в поле Url уже публичную ссылку
            m.Url = _urlResolver.Resolve(m.Url, m.StorageUrl);
        }
    }
}
