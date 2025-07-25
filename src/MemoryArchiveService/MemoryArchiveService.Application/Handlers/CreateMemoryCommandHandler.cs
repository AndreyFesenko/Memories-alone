// src/MemoryArchiveService/MemoryArchiveService.Application/Handlers/CreateMemoryCommandHandler.cs
using MediatR;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Domain.Entities;

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
        // Загружаем файл в Supabase Storage
        var storageUrl = await _storage.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            ct
        );

        // Определение типа медиа
        var mediaType = Enum.TryParse<MediaType>(request.MediaType, ignoreCase: true, out var parsedType)
            ? parsedType
            : MediaType.Image;

        // Создание объекта памяти
        var memory = new Memory
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.TryParse(request.OwnerId, out var ownerId) ? ownerId : throw new ArgumentException("Invalid OwnerId"),

            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            AccessLevel = Enum.TryParse(request.AccessLevel, out AccessLevel level) ? level : AccessLevel.Private,
            Tags = new List<Tag>(),
            MediaFiles = new List<MediaFile>
            {
                new MediaFile
                {
                    Id = Guid.NewGuid(),
                    FileName = request.FileName,
                    Url = storageUrl,
                    StorageUrl = storageUrl,
                    MediaType = mediaType,
                    OwnerId = request.OwnerId,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        // Обработка тегов
        if (request.Tags != null)
        {
            foreach (var tagName in request.Tags)
            {
                var tag = await _tags.GetByNameAsync(tagName, ct)
                    ?? new Tag { Id = Guid.NewGuid(), Name = tagName };
                memory.Tags.Add(tag);
            }
        }

        // Сохраняем в базу
        await _repo.AddAsync(memory, ct);

        // Отправляем событие
        await _eventBus.PublishAsync(new { Event = "MemoryCreated", MemoryId = memory.Id }, ct);

        return new MemoryDto
        {
            Id = memory.Id,
            OwnerId = memory.OwnerId,
            Title = memory.Title,
            Description = memory.Description,
            CreatedAt = memory.CreatedAt,
            AccessLevel = memory.AccessLevel.ToString(),
            Tags = memory.Tags.Select(t => t.Name).ToList()
        };
    }
}
