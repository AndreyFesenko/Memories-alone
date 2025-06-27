// src/NotificationService/NotificationService.Application/DTOs/NotificationMessageDto.cs
using NotificationService.Domain.Entities;

namespace NotificationService.Application.DTOs;

public class NotificationMessageDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string Recipient { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? Title { get; set; }
    public bool IsRead { get; set; }
    public string Type { get; set; } = default!;
    public string Channel { get; set; } = "Email";
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Status { get; set; }
    public string? FailureReason { get; set; }
}
