namespace NotificationService.Application.DTOs;

public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}