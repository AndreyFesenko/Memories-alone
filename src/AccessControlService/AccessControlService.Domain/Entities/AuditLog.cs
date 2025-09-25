#nullable enable
using System;

namespace AccessControlService.Infrastructure.Entities;

/// <summary>
/// Запись аудита операций AccessControl.
/// Храним в схеме БД: access.AuditLogs
/// </summary>
public class AuditLog
{
    public Guid Id { get; set; }

    /// <summary>Короткое имя действия, напр. "GrantAccess" / "RevokeAccess" / "CheckAccess"</summary>
    public string Action { get; set; } = default!;

    /// <summary>Произвольные детали события</summary>
    public string? Details { get; set; }

    /// <summary>Кто совершил (если аутентифицирован)</summary>
    public Guid? UserId { get; set; }

    /// <summary>Время события (UTC)</summary>
    public DateTime CreatedAt { get; set; }
}
