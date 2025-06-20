using MediatR;
using ProfileService.Application.Commands;
using ProfileService.Application.DTOs;
using ProfileService.Application.Interfaces;
using ProfileService.Domain.Entities;

namespace ProfileService.Application.Handlers;

public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, UserProfileDto>
{
    private readonly IProfileRepository _profiles;

    public CreateProfileCommandHandler(IProfileRepository profiles)
    {
        _profiles = profiles;
    }

    public async Task<UserProfileDto> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = new UserProfile
        {
            UserId = request.UserId,
            FullName = request.FullName,
            Bio = request.Bio,
            AvatarUrl = request.AvatarUrl,
            AccessMode = request.AccessMode ?? "AfterDeath",
            DeathConfirmed = false
        };

        await _profiles.CreateAsync(entity, cancellationToken);

        return new UserProfileDto
        {
            UserId = entity.UserId,
            FullName = entity.FullName,
            Bio = entity.Bio,
            AvatarUrl = entity.AvatarUrl,
            AccessMode = entity.AccessMode,
            DeathConfirmed = entity.DeathConfirmed
        };
    }
}
