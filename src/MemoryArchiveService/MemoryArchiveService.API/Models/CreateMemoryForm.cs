// src/MemoryArchiveService/MemoryArchiveService.API/Models/CreateMemoryForm.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MemoryArchiveService.API.Models;

public class CreateMemoryForm
{
    [FromForm] public string OwnerId { get; set; } = default!;
    [FromForm] public string Title { get; set; } = default!;
    [FromForm] public string? Description { get; set; }
    [FromForm] public string MediaType { get; set; } = "Image";
    [FromForm] public string AccessLevel { get; set; } = "Private";
    [FromForm] public List<string>? Tags { get; set; }

    // ВАЖНО: повторяющееся поле "File" → мультизагрузка
    [FromForm(Name = "File")] public List<IFormFile>? Files { get; set; }

    // Опциональный единый alias для имени файла(ов)
    [FromForm] public string? FileName { get; set; }
}
