using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("notifications")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationCommand cmd)
        => Ok(await _mediator.Send(cmd));

    // ...поиск, фильтрация, чтение
}
