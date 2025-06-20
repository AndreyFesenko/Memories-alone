using MediatR;
using ProfileService.Application.DTOs;

namespace ProfileService.Application.Commands 
{
    public class GetProfileQuery : IRequest<UserProfileDto>
    {
        public Guid UserId { get; set; }
    }
}
