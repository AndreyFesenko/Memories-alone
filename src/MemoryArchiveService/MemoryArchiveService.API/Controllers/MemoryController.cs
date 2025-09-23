// src/MemoryArchiveService/MemoryArchiveService.API/Controllers/MemoryController.cs
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

        NormalizeMediaUrls(result);
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
