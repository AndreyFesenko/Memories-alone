using MediatR;

namespace NotificationService.Application.Commands
{
    public class DeleteTemplateCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
