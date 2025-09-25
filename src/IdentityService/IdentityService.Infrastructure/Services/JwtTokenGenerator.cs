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

    public JwtTokenGenerator(IConfiguration config) => _config = config;

    public string GenerateToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        Dictionary<string, string>? customClaims = null
    )
    {
        var jwtSection = _config.GetSection("Jwt");

        var keyString = jwtSection["Key"];
        if (string.IsNullOrWhiteSpace(keyString))
            throw new InvalidOperationException("Jwt:Key is missing in configuration");

        var issuer = jwtSection["Issuer"] ?? "memories-issuer";
        var audience = jwtSection["Audience"] ?? "memories-audience";
        var expiresMinutes = int.TryParse(jwtSection["ExpiresInMinutes"], out var parsed) ? parsed : 60;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),

            // важно: чтобы User.Identity.Name не был null
            new(ClaimTypes.Name, email),
            // опционально: “читаемое” имя для клиентов
            new("preferred_username", email)
        };

        foreach (var role in roles ?? Enumerable.Empty<string>())
        {
            if (!string.IsNullOrWhiteSpace(role))
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (customClaims is not null)
        {
            foreach (var kv in customClaims)
            {
                if (!string.IsNullOrWhiteSpace(kv.Key) && kv.Value is not null)
                    claims.Add(new Claim(kv.Key, kv.Value));
            }
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(expiresMinutes),
            signingCredentials: credentials
        );

        Console.WriteLine($"[JWT] issuer={issuer}, audience={audience}, expires={now.AddMinutes(expiresMinutes):O}");
        foreach (var role in roles ?? Array.Empty<string>())
        {
            Console.WriteLine($"[JWT] add role claim: {role}");
        }

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
