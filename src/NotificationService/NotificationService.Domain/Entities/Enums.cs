// src/NotificationService/NotificationService.Domain/Entities/Enums.cs
namespace NotificationService.Domain.Entities;

public enum NotificationType
{
    Info,
    Warning,
    Alert,
    System
}

public enum NotificationChannel
{
    Email,
    Sms,
    Push,
    Webhook
}

public enum NotificationStatus
{
    Queued,
    Sent,
    Failed,
    Read
}
