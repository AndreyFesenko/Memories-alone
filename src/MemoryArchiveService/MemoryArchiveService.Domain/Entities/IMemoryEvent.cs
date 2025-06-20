// src/MemoryArchiveService/MemoryArchiveService.Domain/Events/IMemoryEvent.cs
namespace MemoryArchiveService.Domain.Events;

public interface IMemoryEvent
{
    Guid MemoryId { get; }
    DateTime OccurredAt { get; }
}
