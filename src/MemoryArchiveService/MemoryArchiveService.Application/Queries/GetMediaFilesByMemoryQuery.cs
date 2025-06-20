using MediatR;
using MemoryArchiveService.Application.DTOs;
using System.Collections.Generic;

namespace MemoryArchiveService.Application.Queries;

public class GetMediaFilesByMemoryQuery : IRequest<List<MediaFileDto>>
{
    public Guid MemoryId { get; set; }
}
