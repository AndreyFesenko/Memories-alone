// src/MemoryArchiveService/MemoryArchiveService.Application/DTOs/MemoryDto.cs
using System;

namespace MemoryArchiveService.Application.DTOs;

public class MemoryDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MediaFileDto> MediaFiles { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string AccessLevel { get; set; } = "Private";
    public int MediaCount => MediaFiles?.Count ?? 0;
}

