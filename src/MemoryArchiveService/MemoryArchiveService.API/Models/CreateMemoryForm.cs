using Microsoft.AspNetCore.Http;

namespace MemoryArchiveService.API.Models;

public class CreateMemoryForm
{
    public string OwnerId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? AccessLevel { get; set; }
    public string MediaType { get; set; } = default!;
    public IFormFile File { get; set; } = default!;
    public IEnumerable<string>? Tags { get; set; }
}
