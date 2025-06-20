using MediatR;
using ProfileService.Application.DTOs;
using ProfileService.Application.Commands;
using ProfileService.Application.Interfaces;

namespace ProfileService.Application.Handlers;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileDto?>
{
    private readonly IProfileRepository _profiles;

    public GetProfileQueryHandler(IProfileRepository profiles)
    {
        _profiles = profiles;
    }

    public async Task<UserProfileDto?> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        // Получаем профиль из репозитория
        var profile = await _profiles.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile == null)
            return null;

        // Маппим доменную сущность в DTO (можно через AutoMapper, здесь вручную)
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
