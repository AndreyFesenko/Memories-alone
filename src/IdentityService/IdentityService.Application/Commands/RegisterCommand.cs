using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Commands;

public class RegisterCommand : IRequest<RegisterResponse>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
        public string? UserName { get; set; }
}
