using MediatR;
using ProfileService.Application.Commands;
using ProfileService.Application.Interfaces;

namespace ProfileService.Application.Handlers;

public class ConfirmDeathCommandHandler : IRequestHandler<ConfirmDeathCommand, Unit>
{
    private readonly IProfileRepository _profiles;

    public ConfirmDeathCommandHandler(IProfileRepository profiles)
    {
        _profiles = profiles;
    }

    public async Task<Unit> Handle(ConfirmDeathCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profiles.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile is null) return Unit.Value;

        profile.DeathConfirmed = true;
        await _profiles.UpdateAsync(profile, cancellationToken);
        return Unit.Value;
    }
}
