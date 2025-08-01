//C:\Users\user\Source\Repos\Memories-alone\src\MemoryArchiveService\MemoryArchiveService.Application\Queries\GetMemoryByIdQuery.cs
using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Queries;

public class GetMemoryByIdQuery : IRequest<MemoryDto>
{
    public Guid Id { get; set; }
}
