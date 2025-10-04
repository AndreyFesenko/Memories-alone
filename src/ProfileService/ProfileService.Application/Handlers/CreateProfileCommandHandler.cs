using MediatR;
using ProfileService.Application.Commands;
using ProfileService.Application.DTOs;
using ProfileService.Application.Interfaces;
using ProfileService.Domain.Entities;

namespace ProfileService.Application.Handlers;

public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, UserProfileDto>
{
    private readonly IProfileRepository _repo;

    public CreateProfileCommandHandler(IProfileRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserProfileDto> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = new UserProfile
        {
            UserId = request.UserId,
            FullName = request.FullName,
            BirthDate = request.BirthDate,
            DisplayName = request.DisplayName,
            Bio = request.Bio,
            AccessMode = request.AccessMode,
            AvatarUrl = request.AvatarUrl
        };

        var created = await _repo.CreateAsync(entity, cancellationToken);

        return new UserProfileDto
        {
            Id = created.Id,
            UserId = created.UserId,
            FullName = created.FullName,
            BirthDate = created.BirthDate,
            DisplayName = created.DisplayName,
            Bio = created.Bio,
            DeathConfirmed = created.DeathConfirmed,
            AccessMode = created.AccessMode,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt,
            AvatarUrl = created.AvatarUrl
        };
    }
}
