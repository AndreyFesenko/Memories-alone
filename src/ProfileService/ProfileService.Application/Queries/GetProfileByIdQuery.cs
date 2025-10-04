using MediatR;
using ProfileService.Application.DTOs;

namespace ProfileService.Application.Queries;

public class GetProfileByIdQuery : IRequest<UserProfileDto?>
{
    public Guid Id { get; set; }
}
