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
        // 1) Загружаем файл (сторедж САМ копирует поток внутрь себя)
        var storageUrl = await _storage.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            ct
        );

        // 2) Тип медиа
        var mediaType = Enum.TryParse<MediaType>(request.MediaType, true, out var parsed)
            ? parsed
            : MediaType.Image;

        // 3) Сущность
        var memory = new Memory
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.Parse(request.OwnerId),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            AccessLevel = Enum.TryParse(request.AccessLevel, true, out AccessLevel level) ? level : AccessLevel.Private,
            Tags = new List<Tag>(),
            MediaFiles = new List<MediaFile>
            {
                new()
                {
                    Id         = Guid.NewGuid(),
                    FileName   = request.FileName,
                    Url        = storageUrl,
                    StorageUrl = storageUrl,
                    MediaType  = mediaType,
                    OwnerId    = request.OwnerId,
                    CreatedAt  = DateTime.UtcNow
                }
            }
        };

        if (request.Tags is { Count: > 0 })
        {
            foreach (var name in request.Tags)
            {
                var tag = await _tags.GetByNameAsync(name, ct) ?? new Tag { Id = Guid.NewGuid(), Name = name };
                memory.Tags.Add(tag);
            }
        }

        await _repo.AddAsync(memory, ct);
        await _eventBus.PublishAsync(new { Event = "MemoryCreated", MemoryId = memory.Id }, ct);

        return new MemoryDto
        {
            Id = memory.Id,
            OwnerId = memory.OwnerId,
            Title = memory.Title,
            Description = memory.Description,
            CreatedAt = memory.CreatedAt,
            AccessLevel = memory.AccessLevel.ToString(),
            Tags = memory.Tags.Select(t => t.Name).ToList(),
            MediaFiles = memory.MediaFiles.Select(m => new MediaFileDto
            {
                Id = m.Id,
                MemoryId = memory.Id,                  // безопасно: даже если в сущности нет m.MemoryId
                FileName = m.FileName,
                Url = m.Url ?? m.StorageUrl ?? string.Empty,
                MediaType = m.MediaType.ToString(),
                UploadedAt = m.CreatedAt,              // если в сущности есть UploadedAt — можешь проставить его
                CreatedAt = m.CreatedAt
            }).ToList(),
            MediaCount = memory.MediaFiles.Count
        };
    }
}
