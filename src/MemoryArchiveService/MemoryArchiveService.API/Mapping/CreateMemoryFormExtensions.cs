// src/MemoryArchiveService/MemoryArchiveService.API/Mapping/CreateMemoryFormExtensions.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MemoryArchiveService.API.Models;
using MemoryArchiveService.Application.Commands;

namespace MemoryArchiveService.API.Mapping;

public static class CreateMemoryFormExtensions
{
    /// <summary>
    /// Маппинг формы multipart → CreateMemoryCommand (мультизагрузка).
    /// ВАЖНО: не закрывать и не dispose'ить входные Streams — это сделает хендлер.
    /// </summary>
    public static Task<CreateMemoryCommand> MapToCommandAsync(this CreateMemoryForm form, CancellationToken ct)
    {
        var cmd = new CreateMemoryCommand
        {
            OwnerId = form.OwnerId,
            Title = form.Title ?? string.Empty,
            Description = form.Description ?? string.Empty,
            AccessLevel = string.IsNullOrWhiteSpace(form.AccessLevel) ? "Private" : form.AccessLevel,
            Tags = form.Tags ?? new List<string>(),
            Files = new List<IncomingFile>()
        };

        // Поддержка мультизагрузки: [FromForm(Name = "File")] List<IFormFile> Files
        if (form.Files != null)
        {
            foreach (var f in form.Files)
            {
                if (f is null || f.Length == 0) continue;

                // Если передан общий FileName — используем его как alias, иначе берем оригинальное имя файла
                var effectiveName = !string.IsNullOrWhiteSpace(form.FileName)
                    ? form.FileName!
                    : (string.IsNullOrWhiteSpace(f.FileName) ? "upload.bin" : f.FileName);

                cmd.Files.Add(new IncomingFile
                {
                    FileName = effectiveName,
                    ContentType = string.IsNullOrWhiteSpace(f.ContentType) ? "application/octet-stream" : f.ContentType,
                    // Логический тип ресурса (Image/Video/Audio/Document)
                    MediaType = string.IsNullOrWhiteSpace(form.MediaType) ? "Image" : form.MediaType!,
                    FileStream = f.OpenReadStream() // НЕ закрываем тут!
                });
            }
        }

        return Task.FromResult(cmd);
    }
}
