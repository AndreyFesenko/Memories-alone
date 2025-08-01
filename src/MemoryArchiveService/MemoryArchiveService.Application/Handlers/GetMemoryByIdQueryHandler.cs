//C:\Users\user\Source\Repos\Memories-alone\src\MemoryArchiveService\MemoryArchiveService.Application\Handlers\GetMemoryByIdQueryHandler.cs
using MediatR;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Application.Queries;

namespace MemoryArchiveService.Application.Handlers;

public class GetMemoryByIdQueryHandler : IRequestHandler<GetMemoryByIdQuery, MemoryDto?>
{
    private readonly IMemoryRepository _repo;

    public GetMemoryByIdQueryHandler(IMemoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<MemoryDto?> Handle(GetMemoryByIdQuery request, CancellationToken ct)
    {
        var memory = await _repo.GetByIdAsync(request.Id, ct);
        if (memory == null) return null;

        return new MemoryDto
        {
            Id = memory.Id,
            OwnerId = memory.OwnerId,
            Title = memory.Title,
            Description = memory.Description,
            CreatedAt = memory.CreatedAt,
            AccessLevel = memory.AccessLevel.ToString(),
            Tags = memory.Tags.Select(t => t.Name).ToList(),
            MediaFiles = memory.MediaFiles.Select(mf => new MediaFileDto
            {
                Id = mf.Id,
                MemoryId = mf.MemoryId,
                FileName = mf.FileName,
                MediaType = mf.MediaType.ToString(),
                Url = mf.Url,
                UploadedAt = mf.UploadedAt
            }).ToList()
        };
    }
}
