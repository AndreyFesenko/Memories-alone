// src/MemoryArchiveService/MemoryArchiveService.API/Mapping/CreateMemoryFormExtensions.cs
using MemoryArchiveService.API.Models;
using MemoryArchiveService.Application.Commands;

namespace MemoryArchiveService.API.Mapping;

public static class CreateMemoryFormExtensions
{
    // ВАЖНО: НЕ использовать using — просто вернуть открытый поток.
    public static Task<CreateMemoryCommand> MapToCommandAsync(this CreateMemoryForm form, CancellationToken ct)
    {
        var cmd = new CreateMemoryCommand
        {
            OwnerId = form.OwnerId,
            Title = form.Title,
            Description = form.Description,
            MediaType = form.MediaType,
            AccessLevel = form.AccessLevel,
            Tags = form.Tags,
            FileName = form.File.FileName,
            ContentType = string.IsNullOrWhiteSpace(form.File.ContentType) ? "application/octet-stream" : form.File.ContentType,
            FileStream = form.File.OpenReadStream() // не закрываем здесь!
        };

        return Task.FromResult(cmd);
    }
}
