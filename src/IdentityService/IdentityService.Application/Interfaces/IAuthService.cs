using IdentityService.Application.DTOs;

namespace IdentityService.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string email, string password);
}