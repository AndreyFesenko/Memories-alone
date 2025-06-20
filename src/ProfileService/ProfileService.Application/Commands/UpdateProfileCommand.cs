using MediatR;
using ProfileService.Application.DTOs;

namespace ProfileService.Application.Commands;

public class UpdateProfileCommand : IRequest<UserProfileDto>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }   
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Bio { get; set; }
    public string? AccessMode { get; set; }
    public string? AvatarUrl { get; set; }
}
