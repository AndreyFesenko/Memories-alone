// src/MemoryArchiveService/MemoryArchiveService.Application/Handlers/CreateMemoryCommandHandler.cs
using MediatR;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace MemoryArchiveService.Application.Handlers;

public class CreateMemoryCommandHandler : IRequestHandler<CreateMemoryCommand, MemoryDto>
{
    private readonly IMemoryRepository _repo;
    private readonly ITagRepository _tags;
    private readonly IEventBus _eventBus;
    private readonly IStorageService _storage;

    public CreateMemoryCommandHandler(
        IMemoryRepository repo,
        ITagRepository tags,
        IEventBus eventBus,
        IStorageService storage)
    {
        _repo = repo;
        _tags = tags;
        _eventBus = eventBus;
        _storage = storage;
    }

    public async Task<MemoryDto> Handle(CreateMemoryCommand request, CancellationToken ct)
    {
        if (request.Files is null || request.Files.Count == 0)
            throw new ValidationException("At least one File must be provided.");

        // Парсим и валидируем OwnerId
        var ownerGuid = Guid.TryParse(request.OwnerId, out var ownerId)
            ? ownerId
            : throw new ArgumentException("Invalid OwnerId");

        // Создаём Memory (метаданные)
        var memory = new Memory
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerGuid,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            AccessLevel = Enum.TryParse(request.AccessLevel, true, out AccessLevel level) ? level : AccessLevel.Private,
            Tags = new List<Tag>(),
            MediaFiles = new List<MediaFile>()
        };

        // Загрузка всех файлов
        foreach (var incoming in request.Files)
        {
            if (incoming?.FileStream == null || string.IsNullOrWhiteSpace(incoming.FileName))
                continue;

            // Буферизуем входной поток (безопасно для повторного чтения на время аплоада)
            await using var buffer = new MemoryStream();
            await incoming.FileStream.CopyToAsync(buffer, ct);
            buffer.Position = 0;

            // Грузим в сторедж (Supabase/S3)
            var upload = await _storage.UploadAsync(
                buffer,
                incoming.FileName,
                string.IsNullOrWhiteSpace(incoming.ContentType) ? "application/octet-stream" : incoming.ContentType,
                ct);

            // Определяем доменный медиа-тип
            var mediaType = Enum.TryParse<MediaType>(incoming.MediaType, ignoreCase: true, out var parsedType)
                ? parsedType
                : GuessMediaTypeFromContentType(incoming.ContentType);

            var media = new MediaFile
            {
                Id = Guid.NewGuid(),
                FileName = incoming.FileName,
                Url = upload.Url,
                StorageUrl = upload.Url,
                MediaType = mediaType,
                // В твоём коде здесь было string OwnerId = request.OwnerId; — оставим такую совместимость:
                OwnerId = request.OwnerId, // если в сущности это string; если Guid — подставь ownerGuid
                CreatedAt = DateTime.UtcNow,
                Size = upload.Size,
                MemoryId = memory.Id
            };

            memory.MediaFiles.Add(media);
        }

        // Теги (переиспользуем твою логику)
        if (request.Tags is { Count: > 0 })
        {
            foreach (var tagName in request.Tags)
            {
                var tag = await _tags.GetByNameAsync(tagName, ct)
                          ?? new Tag { Id = Guid.NewGuid(), Name = tagName };
                memory.Tags.Add(tag);
            }
        }

        await _repo.AddAsync(memory, ct);

        await _eventBus.PublishAsync(new { Event = "MemoryCreated", MemoryId = memory.Id }, ct);

        // Мэппинг в DTO
        var mediaDtos = memory.MediaFiles.Select(m => new MediaFileDto
        {
            Id = m.Id,
            FileName = m.FileName,
            Url = m.Url,
            StorageUrl = m.StorageUrl,
            MediaType = m.MediaType.ToString(),
            OwnerId = m.OwnerId,
            CreatedAt = m.CreatedAt,
            Size = m.Size
        }).ToList();

        return new MemoryDto
        {
            Id = memory.Id,
            OwnerId = memory.OwnerId,
            Title = memory.Title,
            Description = memory.Description,
            CreatedAt = memory.CreatedAt,
            AccessLevel = memory.AccessLevel.ToString(),
            Tags = memory.Tags.Select(t => t.Name).ToList(),
            MediaFiles = mediaDtos,
            MediaCount = mediaDtos.Count
        };
    }

    private static MediaType GuessMediaTypeFromContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType)) return MediaType.Image;

        if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) return MediaType.Image;
        if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)) return MediaType.Video;
        if (contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)) return MediaType.Audio;

        return MediaType.Document;
    }
}
