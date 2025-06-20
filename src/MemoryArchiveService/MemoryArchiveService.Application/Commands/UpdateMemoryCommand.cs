using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Commands;

public class UpdateMemoryCommand : IRequest<MemoryDto>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}
