using MediatR;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Handlers;

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

        if (!string.IsNullOrWhiteSpace(request.Name))
            entity.Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.Body))
            entity.Body = request.Body;
        if (request.Type.HasValue)
            entity.Type = request.Type.Value;

        entity.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(entity, cancellationToken);

        return new NotificationTemplateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Body = entity.Body,
            Type = entity.Type.ToString(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
