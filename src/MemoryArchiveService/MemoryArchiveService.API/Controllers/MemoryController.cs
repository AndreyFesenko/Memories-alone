using Microsoft.AspNetCore.Mvc;
using MediatR;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.Application.Queries;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemoryController : ControllerBase
{
    private readonly IMediator _mediator;
    public MemoryController(IMediator mediator) => _mediator = mediator;

    // Получить память по Id
    [HttpGet("{id}")]
    public async Task<ActionResult<MemoryDto>> Get(Guid id)
        => Ok(await _mediator.Send(new GetMemoryQuery { Id = id }));

    // Получить все воспоминания пользователя
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<MemoryDto>>> GetByUser(Guid userId)
        => Ok(await _mediator.Send(new GetMemoriesByUserQuery { UserId = userId }));

    // Создать память
    [HttpPost]
    public async Task<ActionResult<MemoryDto>> Create([FromBody] CreateMemoryCommand cmd)
        => Ok(await _mediator.Send(cmd));

    // Обновить
    [HttpPut("{id}")]
    public async Task<ActionResult<MemoryDto>> Update(Guid id, [FromBody] UpdateMemoryCommand cmd)
    {
        cmd.Id = id;
        return Ok(await _mediator.Send(cmd));
    }

    // Удалить
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteMemoryCommand { Id = id });
        return NoContent();
    }
}
