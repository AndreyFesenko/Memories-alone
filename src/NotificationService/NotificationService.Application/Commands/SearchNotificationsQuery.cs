using MediatR;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Queries;

public class SearchNotificationsQuery : IRequest<List<NotificationDto>>
{
    public string? Recipient { get; set; }
    public NotificationStatus? Status { get; set; }
    public NotificationType? Type { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
