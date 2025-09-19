//C:\Users\user\Source\Repos\Memories-alone\src\MemoryArchiveService\MemoryArchiveService.Application\DTOs\MediaFileDto.cs
namespace MemoryArchiveService.Application.DTOs;

public sealed class MediaFileDto
{
    public Guid Id { get; set; }

    // NEW: идентификатор памяти, к которой относится файл
    public Guid MemoryId { get; set; }

    public string FileName { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string MediaType { get; set; } = "Image";

    // NEW: когда файл был загружен
    public DateTime UploadedAt { get; set; }

    // Оставим CreatedAt для обратной совместимости (если где-то уже используется)
    public DateTime CreatedAt { get; set; }
}
