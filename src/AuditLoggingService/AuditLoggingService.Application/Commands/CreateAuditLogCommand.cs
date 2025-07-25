using MediatR;
using AuditLoggingService.Application.DTOs;

namespace AuditLoggingService.Application.Commands;

public class CreateAuditLogCommand : IRequest<AuditLogDto>
{
    public string Action { get; set; } = default!;
    public string Target { get; set; } = default!;       // 👈 нужно для поля Target
    public string? Details { get; set; }                 // 👈 может быть null
    public string? Result { get; set; }                  // 👈 может быть null
    public string? Data { get; set; }                    // 👈 для JSON или вложений
    public Guid? UserId { get; set; }                    // 👈 тип должен совпадать с Domain-моделью
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
