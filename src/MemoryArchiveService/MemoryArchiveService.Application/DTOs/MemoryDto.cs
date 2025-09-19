// src/MemoryArchiveService/MemoryArchiveService.Application/DTOs/MemoryDto.cs
using System;

namespace MemoryArchiveService.Application.DTOs;

public sealed class MemoryDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // было: Tags, AccessLevel, MediaCount
    public List<string> Tags { get; set; } = new();
    public string AccessLevel { get; set; } = "Private";

    // НОВОЕ:
    public List<MediaFileDto> MediaFiles { get; set; } = new();
    public int MediaCount { get; set; }
}

