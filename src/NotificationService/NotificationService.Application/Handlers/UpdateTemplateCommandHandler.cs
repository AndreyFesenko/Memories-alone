using MediatR;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Application.Handlers
{
    public class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, NotificationTemplateDto>
    {
        private readonly INotificationTemplateRepository _repo;

        public UpdateTemplateCommandHandler(INotificationTemplateRepository repo)
        {
            _repo = repo;
        }

        public async Task<NotificationTemplateDto> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                throw new Exception("Template not found");

            if (request.Name != null) entity.Name = request.Name;
            if (request.Subject != null) entity.Subject = request.Subject;
            if (request.BodyTemplate != null) entity.BodyTemplate = request.BodyTemplate;
            entity.Type = request.Type;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(entity, cancellationToken);

            return new NotificationTemplateDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Subject = entity.Subject,
                BodyTemplate = entity.BodyTemplate,
                Type = entity.Type,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
