using MediatR;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Handlers;

public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, NotificationTemplateDto>
{
    private readonly INotificationTemplateRepository _repo;

    public CreateTemplateCommandHandler(INotificationTemplateRepository repo)
    {
        _repo = repo;
    }

    public async Task<NotificationTemplateDto> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var entity = new NotificationTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Body = request.Body,
            Type = request.Type,
            CreatedAt = now,
            UpdatedAt = null // либо now, если нужно
        };

        await _repo.AddAsync(entity, cancellationToken);

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
