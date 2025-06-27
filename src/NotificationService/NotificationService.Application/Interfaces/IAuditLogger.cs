// Application/Interfaces/IAuditLogger.cs
public interface IAuditLogger
{
    Task LogAsync(string action, string details, string? userId, CancellationToken ct);
}