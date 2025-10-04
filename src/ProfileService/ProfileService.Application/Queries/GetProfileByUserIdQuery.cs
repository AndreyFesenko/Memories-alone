using MediatR;
using ProfileService.Application.DTOs;

namespace ProfileService.Application.Queries;

public class GetProfileByUserIdQuery : IRequest<UserProfileDto?>
{
    public Guid UserId { get; set; }
}
