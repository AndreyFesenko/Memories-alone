using MediatR;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Interfaces;
using MemoryArchiveService.Application.Queries;

namespace MemoryArchiveService.Application.Handlers;

public class GetMemoriesByUserQueryHandler : IRequestHandler<GetMemoriesByUserQuery, PagedResult<MemoryDto>>
{
    private readonly IMemoryRepository _repo;

    public GetMemoriesByUserQueryHandler(IMemoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<MemoryDto>> Handle(GetMemoriesByUserQuery request, CancellationToken ct)
    {
        var memories = await _repo.GetByOwnerAsync(request.UserId, ct);

        // Фильтрация по AccessLevel
        if (!string.IsNullOrEmpty(request.AccessLevel) &&
            Enum.TryParse<Domain.Entities.AccessLevel>(request.AccessLevel, out var filterLevel))
        {
            memories = memories.Where(m => m.AccessLevel == filterLevel).ToList();
        }

        var totalCount = memories.Count;

        // Пагинация
        var skip = (request.Page - 1) * request.PageSize;
        var pagedMemories = memories.Skip(skip).Take(request.PageSize).ToList();

        return new PagedResult<MemoryDto>
        {
            Items = pagedMemories.Select(memory => new MemoryDto
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
            }).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
