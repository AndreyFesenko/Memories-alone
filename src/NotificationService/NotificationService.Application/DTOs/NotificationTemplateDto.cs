// src/NotificationService/NotificationService.Application/DTOs/NotificationTemplateDto.cs
using NotificationService.Domain.Entities;

namespace NotificationService.Application.DTOs;

public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string BodyTemplate { get; set; } = default!; 
    public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }    
    public DateTime? UpdatedAt { get; set; }
}