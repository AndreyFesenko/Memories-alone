using MediatR;
using MemoryArchiveService.Application.DTOs;

namespace MemoryArchiveService.Application.Queries;

public sealed record GetMediaQuery : IRequest<MediaFileDto?>
{
    public Guid Id { get; init; }
}