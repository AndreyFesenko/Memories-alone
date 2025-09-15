using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("identity")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _cfg;

    public AuthController(IConfiguration cfg) => _cfg = cfg;

    public record LoginRequest(string Username, string Password);
    public record TokenResponse(string AccessToken, string RefreshToken);

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<TokenResponse> Login([FromBody] LoginRequest req)
    {
        // TODO: реальная проверка пользователя (хэш пароля и т.д.)
        if (string.IsNullOrWhiteSpace(req.Username) || req.Password != "pass")
            return Unauthorized();

        return Ok(GenerateTokens(req.Username));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public ActionResult<TokenResponse> Refresh([FromBody] Dictionary<string, string> body)
    {
        // TODO: валидировать refreshToken (хранить в БД/кеше)
        if (!body.TryGetValue("refreshToken", out var refresh) || string.IsNullOrEmpty(refresh))
            return BadRequest("refreshToken required");

        return Ok(GenerateTokens("demo-user"));
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            name = User.Identity?.Name ?? "(no name)",
            claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }

    private TokenResponse GenerateTokens(string username)
    {
        var jwt = _cfg.GetSection("Jwt");
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

        var accessMinutes = int.TryParse(jwt["AccessTokenLifetimeMinutes"], out var m) ? m : 60;
        var refreshDays = int.TryParse(jwt["RefreshTokenLifetimeDays"], out var d) ? d : 7;

        var now = DateTime.UtcNow;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
        };

        var accessToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(accessMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var accessTokenStr = new JwtSecurityTokenHandler().WriteToken(accessToken);
        var refreshToken = Guid.NewGuid().ToString("N"); // TODO: хранить/инвалидировать при logout

        return new TokenResponse(accessTokenStr, refreshToken);
    }
}
