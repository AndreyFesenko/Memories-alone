using MediatR;
using ProfileService.Application.Commands;
using ProfileService.Application.DTOs;
using ProfileService.Application.Interfaces;

namespace ProfileService.Application.Handlers;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    private readonly IProfileRepository _profiles;

    public UpdateProfileCommandHandler(IProfileRepository profiles)
    {
        _profiles = profiles;
    }

    public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profiles.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile == null)
            throw new Exception("Profile not found");

        profile.FullName = request.FullName ?? profile.FullName;
        profile.Bio = request.Bio ?? profile.Bio;
        profile.AvatarUrl = request.AvatarUrl ?? profile.AvatarUrl;
        profile.AccessMode = request.AccessMode ?? profile.AccessMode;

        await _profiles.UpdateAsync(profile, cancellationToken);

        return new UserProfileDto
        {
            UserId = profile.UserId,
            FullName = profile.FullName,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            AccessMode = profile.AccessMode,
            DeathConfirmed = profile.DeathConfirmed
        };
    }
}
