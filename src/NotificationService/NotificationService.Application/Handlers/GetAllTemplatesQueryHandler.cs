using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Application.Queries;
using NotificationService.Application.Interfaces;

namespace NotificationService.Application.Handlers;

public class GetAllTemplatesQueryHandler : IRequestHandler<GetAllTemplatesQuery, List<NotificationTemplateDto>>
{
    private readonly INotificationTemplateRepository _repo;

    public GetAllTemplatesQueryHandler(INotificationTemplateRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<NotificationTemplateDto>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetAllAsync(cancellationToken);
        return list.Select(x => new NotificationTemplateDto
        {
            Id = x.Id,
            Name = x.Name,
            Body = x.Body,
            Type = x.Type.ToString(),
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();
    }
}
