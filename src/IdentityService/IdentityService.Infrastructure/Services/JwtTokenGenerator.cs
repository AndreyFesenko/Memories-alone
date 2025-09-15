// Memories-alone\src\IdentityService\IdentityService.Infrastructure\Services\JwtTokenGenerator.cs
using IdentityService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;

    public JwtTokenGenerator(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        Dictionary<string, string>? customClaims = null
    )
    {
        // ---- Read JWT settings with safe fallbacks ----
        var jwtSection = _config.GetSection("Jwt");

        var keyString = jwtSection["Key"];
        if (string.IsNullOrWhiteSpace(keyString))
            throw new InvalidOperationException("Jwt:Key is missing in configuration");

        var issuer = jwtSection["Issuer"] ?? "memories-issuer";
        var audience = jwtSection["Audience"] ?? "memories-audience";

        var expiresMinutes = 60;
        if (int.TryParse(jwtSection["ExpiresInMinutes"], out var parsed))
            expiresMinutes = parsed;

        // ---- Build security credentials ----
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        // ---- Build claims ----
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var role in roles ?? Enumerable.Empty<string>())
        {
            if (!string.IsNullOrWhiteSpace(role))
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (customClaims != null)
        {
            foreach (var kv in customClaims)
            {
                if (!string.IsNullOrWhiteSpace(kv.Key) && kv.Value is not null)
                    claims.Add(new Claim(kv.Key, kv.Value));
            }
        }

        // ---- Create token ----
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(expiresMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
