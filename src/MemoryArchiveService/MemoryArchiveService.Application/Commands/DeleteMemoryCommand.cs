using MediatR;

namespace MemoryArchiveService.Application.Commands;

public sealed class DeleteMemoryCommand : IRequest<bool>
{
    public Guid Id { get; set; }                 // Id памяти
    public Guid? RequesterId { get; set; }       // кто удаляет (из JWT), если нужно для проверок
}
