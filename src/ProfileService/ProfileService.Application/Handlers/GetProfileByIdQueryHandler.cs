using MediatR;
using ProfileService.Application.DTOs;
using ProfileService.Application.Interfaces;
using ProfileService.Application.Queries;

namespace ProfileService.Application.Handlers;

public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, UserProfileDto?>
{
    private readonly IProfileRepository _repo;

    public GetProfileByIdQueryHandler(IProfileRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserProfileDto?> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (p is null) return null;

        return new UserProfileDto
        {
            Id = p.Id,
            UserId = p.UserId,
            FullName = p.FullName,
            BirthDate = p.BirthDate,
            DisplayName = p.DisplayName,
            Bio = p.Bio,
            DeathConfirmed = p.DeathConfirmed,
            AccessMode = p.AccessMode,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            AvatarUrl = p.AvatarUrl
        };
    }
}
