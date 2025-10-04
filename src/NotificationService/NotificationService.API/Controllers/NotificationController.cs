// src/NotificationService/NotificationService.API/Controllers/NotificationController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("notifications")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly INotificationPusher _pusher;

    public NotificationController(IMediator mediator, INotificationPusher pusher)
    {
        _mediator = mediator;
        _pusher = pusher;
    }

    // Если у тебя есть CQRS-команда создания в БД/шине — оставляем.
    // POST /api/Notification
    [HttpPost]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    // ========= Unified API: отправка одному пользователю (по userId) =========
    public sealed record NotifyUserRequest(
        string UserId,
        string Subject,
        string Body,
        NotificationType Type,
        Dictionary<string, object>? Data,
        string? Channel,
        string? Recipient // опционально, на будущее; ИНТЕРФЕЙС не поддерживает отправку по recipient
    );

    /// <summary>
    /// Отправить уведомление одному пользователю по userId.
    /// POST /api/Notification/notify
    /// </summary>
    [HttpPost("notify")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Notify([FromBody] NotifyUserRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserId))
            return BadRequest(new { error = "userId is required" });

        var msg = new NotificationMessage
        {
            Id = Guid.NewGuid(),
            UserId = req.UserId,
            Recipient = req.Recipient ?? string.Empty, // интерфейс всё равно шлёт по userId
            Subject = req.Subject,
            Body = req.Body,
            Type = req.Type,
            Channel = req.Channel ?? "InApp",
            CreatedAt = DateTime.UtcNow,
            Status = NotificationStatus.Queued
        };

        await _pusher.NotifyUserAsync(req.UserId, msg);
        return Accepted(new { messageId = msg.Id, userId = req.UserId, status = "queued" });
    }

    // ======= Unified API: отправка группе пользователей (по userIds[]) =======
    public sealed record NotifyManyRequest(
        IEnumerable<string> UserIds,
        string Subject,
        string Body,
        NotificationType Type,
        Dictionary<string, object>? Data,
        string? Channel,
        IEnumerable<string>? Recipients // опционально, но НЕ поддерживается данным интерфейсом
    );

    /// <summary>
    /// Отправить уведомление группе по userIds.
    /// POST /api/Notification/notify-many
    /// </summary>
    [HttpPost("notify-many")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> NotifyMany([FromBody] NotifyManyRequest req, CancellationToken ct)
    {
        var userIds = (req.UserIds ?? Array.Empty<string>())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToArray();

        if (userIds.Length == 0)
            return BadRequest(new { error = "Provide at least one userId" });

        var msg = new NotificationMessage
        {
            Id = Guid.NewGuid(),
            // В пакетной отправке сам pusher проставит нужный userId по месту
            Subject = req.Subject,
            Body = req.Body,
            Type = req.Type,
            Channel = req.Channel ?? "InApp",
            CreatedAt = DateTime.UtcNow,
            Status = NotificationStatus.Queued
        };

        await _pusher.NotifyUsersAsync(userIds, msg);
        return Accepted(new { messageId = msg.Id, users = userIds.Length, status = "queued" });
    }

    // ========================= Не поддерживается интерфейсом =========================
    // Broadcast всем и отправка по "recipients" отсутствуют в INotificationPusher.
    // Оставляем заглушки, чтобы UI/Unified API не падали, но честно возвращаем 501.

    public sealed record BroadcastRequest(
        string Subject,
        string Body,
        NotificationType Type,
        Dictionary<string, object>? Data,
        string? Channel
    );

    /// <summary>
    /// NOT IMPLEMENTED: широковещание не поддерживается текущим INotificationPusher.
    /// </summary>
    [HttpPost("broadcast")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult Broadcast([FromBody] BroadcastRequest _)
        => StatusCode(StatusCodes.Status501NotImplemented, new { error = "Broadcast is not supported by current pusher interface." });

    public sealed record RecipientsRequest(
        IEnumerable<string> Recipients,
        string Subject,
        string Body,
        NotificationType Type,
        Dictionary<string, object>? Data,
        string? Channel
    );

    /// <summary>
    /// NOT IMPLEMENTED: отправка по произвольным получателям (email/телефон) не поддерживается текущим INotificationPusher.
    /// </summary>
    [HttpPost("notify-by-recipients")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult NotifyByRecipients([FromBody] RecipientsRequest _)
        => StatusCode(StatusCodes.Status501NotImplemented, new { error = "Sending by recipients is not supported by current pusher interface." });
}
