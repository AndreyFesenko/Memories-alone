using System.Security.Claims;

namespace IdentityService.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        Dictionary<string, string>? customClaims = null
    );
}
