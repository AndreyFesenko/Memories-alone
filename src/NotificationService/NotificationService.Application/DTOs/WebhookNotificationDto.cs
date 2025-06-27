// src/NotificationService/NotificationService.Application/DTOs/WebhookNotificationDto.cs
namespace NotificationService.Application.DTOs;

public class WebhookNotificationDto
{
    public string Url { get; set; } = default!;
    public object Payload { get; set; } = default!;
}
