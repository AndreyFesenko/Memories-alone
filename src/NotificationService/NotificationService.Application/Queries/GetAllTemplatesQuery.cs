// src/NotificationService/NotificationService.Application/Queries/GetAllTemplatesQuery.cs
using MediatR;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Queries;

public class GetAllTemplatesQuery : IRequest<List<NotificationTemplateDto>> { }
