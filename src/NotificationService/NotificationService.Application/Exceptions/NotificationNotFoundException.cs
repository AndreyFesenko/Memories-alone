using System;

namespace NotificationService.Application.Exceptions;

public class NotificationNotFoundException : Exception
{
    public NotificationNotFoundException(Guid id)
        : base($"Уведомление с Id {id} не найдено") { }
}
