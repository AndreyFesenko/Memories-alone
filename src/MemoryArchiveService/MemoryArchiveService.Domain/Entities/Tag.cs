// src/MemoryArchiveService/MemoryArchiveService.Domain/Entities/Tag.cs
namespace MemoryArchiveService.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<MemoryTag> MemoryTags { get; set; } = new List<MemoryTag>();
}