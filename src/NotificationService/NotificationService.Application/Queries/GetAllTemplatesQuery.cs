// src/NotificationService/NotificationService.Application/Queries/GetAllTemplatesQuery.cs
using MediatR;
using NotificationService.Application.DTOs;
using System.Collections.Generic;

namespace NotificationService.Application.Queries
{
    // Запрос — вернуть все шаблоны (или с фильтрацией)
    public class GetAllTemplatesQuery : IRequest<List<NotificationTemplateDto>>
    {
        // Для расширения: public string? Filter { get; set; }
    }
}
