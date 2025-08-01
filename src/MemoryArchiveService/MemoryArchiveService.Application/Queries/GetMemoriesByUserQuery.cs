using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Queries;

public class GetMemoriesByUserQuery : IRequest<PagedResult<MemoryDto>>
{
    public Guid UserId { get; set; }
    public string? AccessLevel { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
