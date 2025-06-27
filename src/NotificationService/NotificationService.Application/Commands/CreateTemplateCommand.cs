using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Commands
{
    public class CreateTemplateCommand : IRequest<NotificationTemplateDto>
    {
        public string Name { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
