using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Commands;

public class CreateNotificationCommand : IRequest<NotificationDto>
{
    public string UserId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string Channel { get; set; } = "Email";
}
