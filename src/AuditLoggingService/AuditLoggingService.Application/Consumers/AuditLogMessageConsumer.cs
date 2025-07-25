//C:\Users\user\source\repos\Memories-alone\src\AuditLoggingService\AuditLoggingService.Application\Consumers\AuditLogMessageConsumer.cs
using Shared.Messaging.Messages;
using AuditLoggingService.Domain.Entities;
using AuditLoggingService.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

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
        var msg = context.Message;

        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = msg.UserId,
            Action = msg.Action,
            Target = msg.Target,
            Data = msg.Data,
            IpAddress = msg.IpAddress,
            UserAgent = msg.UserAgent,
            Timestamp = msg.Timestamp
        };

        await _db.AuditLogs.AddAsync(log);
        await _db.SaveChangesAsync();
    }
}
