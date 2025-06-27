using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Queries
{
    public class GetTemplateQuery : IRequest<NotificationTemplateDto?>
    {
        public Guid Id { get; set; }
    }
}
