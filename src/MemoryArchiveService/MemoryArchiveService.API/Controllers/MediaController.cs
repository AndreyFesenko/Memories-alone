// src\MemoryArchiveService\MemoryArchiveService.API\Controllers\MediaController.cs
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

    // Загрузка файла (multipart/form-data)
    [HttpPost("upload")]
    public async Task<ActionResult<string>> Upload([FromForm] UploadMediaCommand cmd)
        => Ok(await _mediator.Send(cmd));

    // Получить ссылку/файл по Id
    [HttpGet("{id}")]
    public async Task<ActionResult<MediaFileDto>> Get(Guid id)
        => Ok(await _mediator.Send(new GetMediaQuery { Id = id }));

    // Можно добавить выдачу самих файлов, если не используешь ссылки
}
