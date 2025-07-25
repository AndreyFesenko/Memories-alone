// src/MemoryArchiveService/MemoryArchiveService.Application/Commands/CreateMemoryCommand.cs
using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Commands;

public class CreateMemoryCommand : IRequest<MemoryDto>
{
    public string OwnerId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string AccessLevel { get; set; } = "Private";
    public List<string>? Tags { get; set; }

    // Загрузка файла
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string MediaType { get; set; } = default!;
    public Stream FileStream { get; set; } = default!;
}