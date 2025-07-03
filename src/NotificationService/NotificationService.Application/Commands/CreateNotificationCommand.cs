using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Commands;

public class CreateNotificationCommand : IRequest<NotificationDto>
{
    public string Recipient { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string Channel { get; set; } = "Email";
}
