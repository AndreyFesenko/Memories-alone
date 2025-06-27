// src/NotificationService/NotificationService.Application/Commands/UpdateTemplateCommand.cs
using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Commands;

public class UpdateTemplateCommand : IRequest<NotificationTemplateDto>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Body { get; set; }
    public NotificationType? Type { get; set; }
}
