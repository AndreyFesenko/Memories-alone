using MediatR;
using ProfileService.Application.Commands;
using ProfileService.Application.Interfaces;

namespace ProfileService.Application.Handlers;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Unit>
{
    private readonly IProfileRepository _profiles;

    public DeleteProfileCommandHandler(IProfileRepository profiles)
    {
        _profiles = profiles;
    }

    public async Task<Unit> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        await _profiles.DeleteAsync(request.UserId, cancellationToken);
        return Unit.Value;
    }
}
