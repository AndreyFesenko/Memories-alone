using MediatR;

namespace MemoryArchiveService.Application.Commands;

public class DeleteMediaFileCommand : IRequest
{
    public Guid Id { get; set; } // MediaFile Id
}
