// src/NotificationService/NotificationService.Application/DTOs/NotificationTemplateDto.cs

namespace NotificationService.Application.DTOs;

public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string Type { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

