// src/NotificationService/NotificationService.Application/Handlers/CreateTemplateCommandHandler.cs

using AutoMapper;
using MediatR;
using NotificationService.Application.Commands;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Handlers;

public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, NotificationTemplateDto>
{
    private readonly INotificationTemplateRepository _repo;
    private readonly IMapper _mapper;

    public CreateTemplateCommandHandler(INotificationTemplateRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<NotificationTemplateDto> Handle(CreateTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = new NotificationTemplate
        {
            Name = command.Name,
            Subject = command.Subject,
            BodyTemplate = command.BodyTemplate, 
            Type = command.Type
        };

        await _repo.AddAsync(template, cancellationToken);

        return _mapper.Map<NotificationTemplateDto>(template);
    }
}
