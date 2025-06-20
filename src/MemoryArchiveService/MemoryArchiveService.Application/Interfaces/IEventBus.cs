// src/MemoryArchiveService/MemoryArchiveService.Application/Interfaces/IEventBus.cs
namespace MemoryArchiveService.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class;
}
