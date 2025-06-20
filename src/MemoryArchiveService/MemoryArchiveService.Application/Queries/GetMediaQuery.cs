using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Queries;

public class GetMediaQuery : IRequest<MediaFileDto>
{
    public Guid Id { get; set; }
}
