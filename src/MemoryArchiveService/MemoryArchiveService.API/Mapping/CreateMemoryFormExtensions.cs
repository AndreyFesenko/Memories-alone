// src/MemoryArchiveService/MemoryArchiveService.API/Mapping/CreateMemoryFormExtensions.cs
using MemoryArchiveService.API.Models;
using MemoryArchiveService.Application.Commands;
using Microsoft.AspNetCore.Http;

namespace MemoryArchiveService.API.Mapping;

public static class CreateMemoryFormExtensions
{
    public static async Task<CreateMemoryCommand> MapToCommandAsync(this CreateMemoryForm form, CancellationToken ct = default)
    {
        using var stream = new MemoryStream();
        await form.File.CopyToAsync(stream, ct);
        stream.Position = 0;

        return new CreateMemoryCommand
        {
            OwnerId = form.OwnerId,
            Title = form.Title,
            Description = form.Description,
            AccessLevel = form.AccessLevel,
            Tags = form.Tags?.ToList() ?? new List<string>(),
            FileName = form.File.FileName,
            ContentType = form.File.ContentType,
            FileStream = stream,
            MediaType = form.File.ContentType.StartsWith("video") ? "Video"
                       : form.File.ContentType.StartsWith("audio") ? "Audio"
                       : "Image"
        };
    }
}
