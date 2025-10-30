// MemoryArchiveService.API/Contracts/CreateMemoryForm.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MemoryArchiveService.API.Contracts;

public class CreateMemoryForm
{
    [FromForm, Required] public Guid OwnerId { get; set; }
    [FromForm, Required] public string Title { get; set; } = default!;
    [FromForm] public string? Description { get; set; }

    // оставляем как у тебя (enum-ы)
    [FromForm, Required] public string MediaType { get; set; } = "Image";
    [FromForm, Required] public string AccessLevel { get; set; } = "Private";

    // Повторяемое поле Tags (много значений через repeatable inputs)
    [FromForm] public List<string> Tags { get; set; } = new();

    // ВАЖНО: принимаем несколько частей с именем "File"
    [FromForm(Name = "File")] public List<IFormFile> Files { get; set; } = new();

    // Для обратной совместимости, когда фронт присылает FileName (один)
    [FromForm] public string? FileName { get; set; }
}
