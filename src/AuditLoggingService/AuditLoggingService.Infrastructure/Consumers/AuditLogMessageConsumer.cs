using Shared.Messaging.Messages;
using AuditLoggingService.Domain.Entities;
using AuditLoggingService.Infrastructure.Persistence;
using MassTransit;

namespace AuditLoggingService.Application.Consumers;

public class AuditLogMessageConsumer : IConsumer<AuditLogMessage>
{
    private readonly AuditLoggingDbContext _db;

    public AuditLogMessageConsumer(AuditLoggingDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<AuditLogMessage> context)
    {
        var m = context.Message;

        var entity = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = Guid.TryParse(m.UserId, out var uid) ? uid : (Guid?)null,
            Action = m.Action,
            Target = m.Target,
            Data = m.Data,
            IpAddress = m.IpAddress,
            UserAgent = m.UserAgent,

            // если в сообщении не проставлен валидный момент события
            Timestamp = (m.Timestamp == default) ? DateTime.UtcNow : m.Timestamp,
            CreatedAt = DateTime.UtcNow
        };

        await _db.AuditLogs.AddAsync(entity, context.CancellationToken);
        await _db.SaveChangesAsync(context.CancellationToken);
    }
}
