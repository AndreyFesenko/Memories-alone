// src/MemoryArchiveService/MemoryArchiveService.Application/Commands/CreateMemoryCommand.cs
using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Commands;

public class CreateMemoryCommand : IRequest<MemoryDto>
{
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public string AccessLevel { get; set; } = "Private";
}
