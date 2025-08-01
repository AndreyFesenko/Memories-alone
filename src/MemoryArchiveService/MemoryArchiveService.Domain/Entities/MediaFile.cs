//C:\Users\user\Source\Repos\Memories-alone\src\MemoryArchiveService\MemoryArchiveService.Domain\Entities\MediaFile.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoryArchiveService.Domain.Entities;

/// <summary>
/// Медиафайл, прикреплённый к воспоминанию
/// </summary>
public class MediaFile
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid MemoryId { get; set; }

    [ForeignKey("MemoryId")]
    public Memory Memory { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = default!;

    [Required]
    [MaxLength(1000)]
    public string Url { get; set; } = default!;

    [Required]
    public MediaType MediaType { get; set; } = MediaType.Other;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(64)]
    public string OwnerId { get; set; } = default!;

    [Required]
    [MaxLength(2000)]
    public string StorageUrl { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public long Size { get; set; }
}

public enum MediaType
{
    Image,
    Video,
    Audio,
    Document,
    Other
}
