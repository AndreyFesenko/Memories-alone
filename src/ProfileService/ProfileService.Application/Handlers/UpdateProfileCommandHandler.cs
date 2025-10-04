using MediatR;
using ProfileService.Application.Commands;
using ProfileService.Application.DTOs;
using ProfileService.Application.Interfaces;

namespace ProfileService.Application.Handlers;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto?>
{
    private readonly IProfileRepository _repo;

    public UpdateProfileCommandHandler(IProfileRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserProfileDto?> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null) return null;

        existing.FullName = request.FullName;
        existing.BirthDate = request.BirthDate;
        existing.DisplayName = request.DisplayName;
        existing.Bio = request.Bio;
        existing.AccessMode = request.AccessMode;
        existing.AvatarUrl = request.AvatarUrl;

        await _repo.UpdateAsync(existing, cancellationToken);

        return new UserProfileDto
        {
            Id = existing.Id,
            UserId = existing.UserId,
            FullName = existing.FullName,
            BirthDate = existing.BirthDate,
            DisplayName = existing.DisplayName,
            Bio = existing.Bio,
            DeathConfirmed = existing.DeathConfirmed,
            AccessMode = existing.AccessMode,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = existing.UpdatedAt,
            AvatarUrl = existing.AvatarUrl
        };
    }
}
