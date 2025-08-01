
namespace MemoryArchiveService.Domain.Entities;

public class MemoryTag
{
    public Guid MemoryId { get; set; }
    public Memory Memory { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}