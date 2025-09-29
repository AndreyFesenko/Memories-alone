using MediatR;

namespace AuditLoggingService.Application.Commands;

public readonly record struct DeleteAuditLogCommand(Guid Id) : IRequest;
