using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Services;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("notifications")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly SignalRNotificationSender _signalRSender;

    public NotificationController(IMediator mediator, SignalRNotificationSender signalRSender)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _signalRSender = signalRSender ?? throw new ArgumentNullException(nameof(signalRSender));
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationCommand cmd)
        => Ok(await _mediator.Send(cmd));

    [HttpPost("notify")]
    public async Task<IActionResult> Notify([FromBody] NotificationMessage message)
    {
        await _signalRSender.NotifyUserAsync(message.UserId, message);
        return Ok();
    }

    // Пример ручки для поиска/чтения уведомлений
    // [HttpGet]
    // public async Task<ActionResult<List<NotificationDto>>> GetAll() { ... }
}
