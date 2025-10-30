// src/MemoryArchiveService/MemoryArchiveService.Application/Handlers/DeleteMemoryCommandHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.Application.Interfaces;

namespace MemoryArchiveService.Application.Handlers;

public sealed class DeleteMemoryCommandHandler : IRequestHandler<DeleteMemoryCommand, bool>
{
    private readonly IMemoryRepository _repo;

    public DeleteMemoryCommandHandler(IMemoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(DeleteMemoryCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return false;

        // Проверка владельца (если нужно):
        // if (request.RequesterId.HasValue && entity.OwnerId != request.RequesterId.Value)
        //     throw new UnauthorizedAccessException("Недостаточно прав для удаления.");

        await _repo.DeleteAsync(entity.Id, ct);

        // Если в будущем понадобится — можно добавить публикацию события или очистку стораджа.
        // await _eventBus.PublishAsync(new { Event = "MemoryDeleted", MemoryId = entity.Id }, ct);
        // foreach (var mf in entity.MediaFiles)
        //     await _storage.DeleteAsync(mf.StorageUrl ?? mf.Url, ct);

        return true;
    }
}
