// src/MemoryArchiveService/MemoryArchiveService.Domain/Entities/Memory.cs
namespace MemoryArchiveService.Domain.Entities;

public class Memory
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<MediaFile> MediaFiles { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Private;
    public ICollection<MemoryTag> MemoryTags { get; set; } = new List<MemoryTag>();

}

public enum AccessLevel
{
    Private,
    FriendsOnly,
    Public,
    Custom
}
