using MediatR;

namespace MemoryArchiveService.Application.Commands;

public class DeleteMemoryCommand : IRequest
{
    public Guid Id { get; set; }
}
