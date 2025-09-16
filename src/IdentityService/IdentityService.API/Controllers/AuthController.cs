using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityService.Application.Interfaces; // IJwtTokenGenerator

namespace IdentityService.API.Controllers;

[ApiController]
[Route("identity")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _cfg;
    private readonly MemoriesDbContext _db;
    private readonly IJwtTokenGenerator _jwt;

    public AuthController(IConfiguration cfg, MemoriesDbContext db, IJwtTokenGenerator jwt)
    {
        _cfg = cfg;
        _db = db;
        _jwt = jwt;
    }

    // ===== DTOs =====
    public sealed class RegisterRequest
    {
        [Required, MinLength(3)]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        // Ранее был DisplayName — в твоей сущности User его нет. Оставляю опционально,
        // но в БД не сохраняю, чтобы не ломать компиляцию.
        public string? DisplayName { get; set; }
    }

    public record LoginRequest([Required] string Username, [Required] string Password);

    public sealed class RefreshRequest
    {
        [Required] public string RefreshToken { get; set; } = null!;
    }

    public sealed class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string? RefreshToken { get; set; }
    }
    // ===============

    /// <summary>Регистрация нового пользователя</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // дубликаты
        var emailExists = await _db.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (emailExists) return Conflict(new { message = "Email already registered" });

        var usernameExists = await _db.Users.AnyAsync(u => u.UserName == req.Username, ct);
        if (usernameExists) return Conflict(new { message = "Username already taken" });

        // хэш пароля (в проде лучше BCrypt/Argon2)
        var pwdHash = HashPasswordSha256(req.Password);

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = req.Email,
            UserName = req.Username,
            PasswordHash = pwdHash,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        // TODO: если есть таблицы Roles/UserRoles — здесь добавить базовую роль "User"

        // роли для токена (пока пусто, чтобы не гадать по схеме)
        var roles = Array.Empty<string>();
        var access = _jwt.GenerateToken(user.Id, user.Email!, roles);

        return Created($"/identity/users/{user.Id}", new TokenResponse
        {
            AccessToken = access,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.TryParse(_cfg["Jwt:ExpiresInMinutes"], out var m) ? m : 60)
        });
    }

    /// <summary>Логин по username/password</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Username and password are required.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == req.Username, ct);
        if (user is null) return Unauthorized();

        var incomingHash = HashPasswordSha256(req.Password);
        if (!CryptEquals(incomingHash, user.PasswordHash ?? string.Empty))
            return Unauthorized();

        // TODO: вытянуть роли пользователя, если есть схема ролей
        var roles = Array.Empty<string>();
        var token = _jwt.GenerateToken(user.Id, user.Email!, roles);

        return Ok(new TokenResponse
        {
            AccessToken = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.TryParse(_cfg["Jwt:ExpiresInMinutes"], out var m) ? m : 60)
        });
    }

    /// <summary>Обновление access-токена по refresh-токену</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public ActionResult<TokenResponse> Refresh([FromBody] RefreshRequest body)
    {
        // TODO: подключить IRefreshTokenService и валидировать refresh token из БД/кеша.
        if (string.IsNullOrEmpty(body.RefreshToken))
            return BadRequest("refreshToken required");

        // демо-логика
        var demoUserId = Guid.NewGuid();
        var token = _jwt.GenerateToken(demoUserId, "demo@example.com", Array.Empty<string>());

        return Ok(new TokenResponse
        {
            AccessToken = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(
                int.TryParse(_cfg["Jwt:ExpiresInMinutes"], out var m) ? m : 60),
            RefreshToken = body.RefreshToken
        });
    }

    /// <summary>Текущий пользователь (из JWT)</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? User.Identity?.Name
                     ?? "(unknown)";

        return Ok(new
        {
            id = subject,
            name = User.Identity?.Name,
            claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }

    // ===== helpers =====
    private static string HashPasswordSha256(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private static bool CryptEquals(string a, string b)
    {
        if (a is null || b is null || a.Length != b.Length) return false;
        var diff = 0;
        for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
        return diff == 0;
    }
}
