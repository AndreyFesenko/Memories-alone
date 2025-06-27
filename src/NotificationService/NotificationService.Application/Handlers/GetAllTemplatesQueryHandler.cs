// src/NotificationService/NotificationService.Application/Handlers/GetAllTemplatesQueryHandler.cs
using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Application.Handlers
{
    public class GetAllTemplatesQueryHandler : IRequestHandler<GetAllTemplatesQuery, List<NotificationTemplateDto>>
    {
        private readonly INotificationTemplateRepository _repo;

        public GetAllTemplatesQueryHandler(INotificationTemplateRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<NotificationTemplateDto>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repo.GetAllAsync(cancellationToken);
            // Mapping: если нужно, маппь на Dto через AutoMapper или вручную
            return templates.Select(x => new NotificationTemplateDto
            {
                Id = x.Id,
                Name = x.Name,
                Subject = x.Subject,
                Body = x.Body,
                Type = x.Type,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }
    }
}
