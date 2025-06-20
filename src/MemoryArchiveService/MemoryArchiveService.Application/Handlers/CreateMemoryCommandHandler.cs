// src/MemoryArchiveService/MemoryArchiveService.Application/Handlers/CreateMemoryCommandHandler.cs
using MediatR;
using MemoryArchiveService.Application.Commands;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Domain.Entities;

public class CreateMemoryCommandHandler : IRequestHandler<CreateMemoryCommand, MemoryDto>
{
    private readonly IMemoryRepository _repo;
    private readonly ITagRepository _tags;
    private readonly IEventBus _eventBus;

    public CreateMemoryCommandHandler(IMemoryRepository repo, ITagRepository tags, IEventBus eventBus)
    {
        _repo = repo;
        _tags = tags;
        _eventBus = eventBus;
    }

    public async Task<MemoryDto> Handle(CreateMemoryCommand request, CancellationToken ct)
    {
        var memory = new Memory
        {
            Id = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            AccessLevel = Enum.TryParse(request.AccessLevel, out AccessLevel level) ? level : AccessLevel.Private,
            Tags = new List<Tag>()
        };

        if (request.Tags != null)
        {
            foreach (var tagName in request.Tags)
            {
                var tag = await _tags.GetByNameAsync(tagName, ct) ?? new Tag { Id = Guid.NewGuid(), Name = tagName };
                memory.Tags.Add(tag);
            }
        }

        await _repo.AddAsync(memory, ct);

        // Паблишим событие (RabbitMQ реализация)
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
