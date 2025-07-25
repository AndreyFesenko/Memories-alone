// Пример: src/MemoryArchiveService/MemoryArchiveService.API/Models/CreateMemoryForm.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MemoryArchiveService.API.Models;

public class CreateMemoryForm
{
    [Required]
    public string OwnerId { get; set; } = default!;

    [Required]
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public string AccessLevel { get; set; } = "Private";

    public List<string>? Tags { get; set; }

    [Required]
    public IFormFile File { get; set; } = default!;
}
