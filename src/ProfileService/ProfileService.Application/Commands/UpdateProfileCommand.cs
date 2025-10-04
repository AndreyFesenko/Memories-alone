using MediatR;
using ProfileService.Application.DTOs;

namespace ProfileService.Application.Commands;

public class UpdateProfileCommand : IRequest<UserProfileDto?>
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public DateTime? BirthDate { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string AccessMode { get; set; } = "AfterDeath";
    public string? AvatarUrl { get; set; }
}
