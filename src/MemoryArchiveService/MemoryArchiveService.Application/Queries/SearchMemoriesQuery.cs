using MediatR;
using MemoryArchiveService.Application.DTOs;
using System.Collections.Generic;

namespace MemoryArchiveService.Application.Queries;

public class SearchMemoriesQuery : IRequest<PagedMemoriesResult>
{
    // Фильтры
    public Guid? UserId { get; set; }
    public string? TitleContains { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string? MediaType { get; set; } // "photo", "video", "audio", ...

    // Пагинация
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    // Сортировка
    public string? SortBy { get; set; } = "CreatedAt"; // или "Title"
    public bool Desc { get; set; } = true;
}

public class PagedMemoriesResult
{
    public List<MemoryDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
