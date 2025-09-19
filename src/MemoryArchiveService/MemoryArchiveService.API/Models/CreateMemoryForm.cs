// src/MemoryArchiveService/MemoryArchiveService.API/Models/CreateMemoryForm.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MemoryArchiveService.API.Models;

public sealed class CreateMemoryForm
{
    [Required] public string OwnerId { get; set; } = null!;
    [Required] public string Title { get; set; } = null!;
    [Required] public string Description { get; set; } = null!;
    [Required] public string MediaType { get; set; } = "Image";
    public string AccessLevel { get; set; } = "Private";
    public List<string>? Tags { get; set; }

    [Required] public IFormFile File { get; set; } = null!;
}