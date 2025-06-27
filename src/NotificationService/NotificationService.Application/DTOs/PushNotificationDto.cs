// src/NotificationService/NotificationService.Application/DTOs/PushNotificationDto.cs
namespace NotificationService.Application.DTOs;

public class PushNotificationDto
{
    public string UserId { get; set; } = default!;
    public string Message { get; set; } = default!;
}
