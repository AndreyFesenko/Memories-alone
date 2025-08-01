//C:\Users\user\Source\Repos\Memories-alone\src\MemoryArchiveService\MemoryArchiveService.Application\DTOs\MediaFileDto.cs
namespace MemoryArchiveService.Application.DTOs;

public class MediaFileDto
{
    public Guid Id { get; set; }
    public Guid MemoryId { get; set; }
    public string FileName { get; set; } = default!;
    public string MediaType { get; set; } = default!;
    public string Url { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
}
