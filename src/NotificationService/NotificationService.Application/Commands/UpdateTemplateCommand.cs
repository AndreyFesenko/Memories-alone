// src/NotificationService/NotificationService.Application/Commands/UpdateTemplateCommand.cs
using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Commands;

public class UpdateTemplateCommand : IRequest<NotificationTemplateDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string BodyTemplate { get; set; } = default!;
    public NotificationType Type { get; set; }
}
