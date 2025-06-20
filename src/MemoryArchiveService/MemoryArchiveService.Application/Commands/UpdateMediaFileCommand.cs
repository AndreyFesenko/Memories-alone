using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Commands;

public class UpdateMediaFileCommand : IRequest<MediaFileDto>
{
    public Guid Id { get; set; }
    public string? FileName { get; set; }
    public string? MediaType { get; set; }
}
