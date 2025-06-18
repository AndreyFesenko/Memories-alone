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
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60"));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        if (customClaims != null)
        {
            foreach (var kvp in customClaims)
                claims.Add(new Claim(kvp.Key, kvp.Value));
        }

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expires,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
