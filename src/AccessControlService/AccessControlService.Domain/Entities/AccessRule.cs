#nullable enable
using System;

namespace AccessControlService.Infrastructure.Entities;

/// <summary>
/// Правило доступа (ACL-строка).
/// Храним в схеме БД: access.AccessRules
/// </summary>
public class AccessRule
{
    public Guid Id { get; set; }

    /// <summary>Тип субъекта: "User" | "Role"</summary>
    public string SubjectType { get; set; } = default!;

    /// <summary>Идентификатор субъекта. Для User — Guid в строке; для Role — имя роли.</summary>
    public string SubjectId { get; set; } = default!;

    /// <summary>Тип ресурса: напр., "Memory"</summary>
    public string ResourceType { get; set; } = default!;

    /// <summary>Идентификатор ресурса</summary>
    public Guid ResourceId { get; set; }

    /// <summary>Право: "View" | "Edit" | "Delete"</summary>
    public string Permission { get; set; } = default!;

    /// <summary>Когда истекает доступ (если задан)</summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>Кем выдано (пользователь), если известно</summary>
    public Guid? GrantedBy { get; set; }

    /// <summary>Когда создано правило</summary>
    public DateTime CreatedAt { get; set; }
}
