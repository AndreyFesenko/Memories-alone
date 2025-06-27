using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Commands
{
    public class UpdateTemplateCommand : IRequest<NotificationTemplateDto>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? Type { get; set; }
    }
}
