using MediatR;

namespace ProfileService.Application.Commands;

public class DeleteProfileCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
