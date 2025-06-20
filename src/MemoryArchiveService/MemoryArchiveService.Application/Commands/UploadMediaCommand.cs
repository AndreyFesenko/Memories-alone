using MediatR;
using Microsoft.AspNetCore.Http;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Commands;

public class UploadMediaCommand : IRequest<string> // возвращаем URL/Id файла
{
    public Guid MemoryId { get; set; }
    public IFormFile File { get; set; } = default!;
    public string MediaType { get; set; } = default!; // "photo", "video", "audio" и т.д.
}
