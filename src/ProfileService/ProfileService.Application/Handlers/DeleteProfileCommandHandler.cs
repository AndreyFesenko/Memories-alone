using MediatR;
using ProfileService.Application.Commands;
using ProfileService.Application.Interfaces;

namespace ProfileService.Application.Handlers;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Unit>
{
    private readonly IProfileRepository _repo;

    public DeleteProfileCommandHandler(IProfileRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        await _repo.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
