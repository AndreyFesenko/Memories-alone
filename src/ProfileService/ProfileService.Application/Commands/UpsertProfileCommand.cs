using MediatR;
using ProfileService.Application.DTOs;

namespace ProfileService.Application.Commands;

public class UpsertProfileCommand : IRequest<UserProfileDto>
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Bio { get; set; }
    public string? AccessMode { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
}
