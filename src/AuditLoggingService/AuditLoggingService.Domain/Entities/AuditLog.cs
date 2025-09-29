namespace AuditLoggingService.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }

    // Пользователь, инициировавший действие
    public Guid? UserId { get; set; }

    // Тип действия, например: Create, Update, Delete
    public string Action { get; set; } = default!;

    // Целевой объект действия (например, сущность или ID)
    public string Target { get; set; } = default!;

    // Дополнительные данные в JSON (если есть)
    public string? Data { get; set; }

    // Подробности в человекочитаемом формате (если нужно)
    public string? Details { get; set; }

    // Результат выполнения действия (Success, Failure, Warning и т.д.)
    public string? Result { get; set; }

    // IP-адрес инициатора
    public string? IpAddress { get; set; }

    // User-Agent из запроса
    public string? UserAgent { get; set; }

    // Время действия
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Время логирования в БД
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
