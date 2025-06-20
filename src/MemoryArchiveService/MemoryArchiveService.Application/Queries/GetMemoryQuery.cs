using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Queries;

public class GetMemoryQuery : IRequest<MemoryDto>
{
    public Guid Id { get; set; }
}
