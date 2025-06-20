using MediatR;
using MemoryArchiveService.Application.DTOs;
using System.Collections.Generic;

namespace MemoryArchiveService.Application.Queries;

public class GetMemoriesByUserQuery : IRequest<List<MemoryDto>>
{
    public Guid UserId { get; set; }
}
