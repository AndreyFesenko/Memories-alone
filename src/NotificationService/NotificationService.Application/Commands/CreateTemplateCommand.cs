// src/NotificationService/NotificationService.Application/Commands/CreateTemplateCommand.cs
using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Commands;

public class CreateTemplateCommand : IRequest<NotificationTemplateDto>
{
    public string Name { get; set; } = default!;
    public string Subject { get; set; } = default!;  
    public string Body { get; set; } = default!;
    public NotificationType Type { get; set; }
}