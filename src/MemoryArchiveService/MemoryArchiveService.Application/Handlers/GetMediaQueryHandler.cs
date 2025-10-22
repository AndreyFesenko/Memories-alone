// src/.../Application/Handlers/GetMediaQueryHandler.cs
using MediatR;
using MemoryArchiveService.Application.DTOs;
using MemoryArchiveService.Application.Queries;
using MemoryArchiveService.Application.Interfaces; // <-- оставляем этот using

namespace MemoryArchiveService.Application.Handlers;

public sealed class GetMediaQueryHandler : IRequestHandler<GetMediaQuery, MediaFileDto?>
{
    private readonly IMediaReadStore _readStore;

    public GetMediaQueryHandler(IMediaReadStore readStore) => _readStore = readStore;

    public Task<MediaFileDto?> Handle(GetMediaQuery request, CancellationToken ct)
        => _readStore.GetByIdAsync(request.Id, ct);
}