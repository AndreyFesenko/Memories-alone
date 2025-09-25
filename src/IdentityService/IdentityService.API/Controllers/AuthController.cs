// C:\_C_Sharp\MyOtus_Prof\Memories_alone\src\IdentityService\IdentityService.API\Controllers\AuthController.cs
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityService.Application.Interfaces; // IJwtTokenGenerator, IUserRepository, IRoleRepository

namespace IdentityService.API.Controllers;

[ApiController]
[Route("identity")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _cfg;
    private readonly MemoriesDbContext _db;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;

    public AuthController(
        IConfiguration cfg,
        MemoriesDbContext db,
        IJwtTokenGenerator jwt,
        IUserRepository users,
        IRoleRepository roles)
    {
        _cfg = cfg;
        _db = db;
        _jwt = jwt;
        _users = users;
        _roles = roles;
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

        // Опционально. В сущности User поля нет — не сохраняем.
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

        // проверки на дубликаты
        if (await _users.EmailExistsAsync(req.Email, ct))
            return Conflict(new { message = "Email already registered" });

        if (await _db.Users.AnyAsync(u => u.UserName == req.Username, ct))
            return Conflict(new { message = "Username already taken" });

        // хэш пароля (в проде лучше BCrypt/Argon2)
        var pwdHash = HashPasswordSha256(req.Password);

        var user = new IdentityService.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = req.Email,
            UserName = req.Username,
            PasswordHash = pwdHash,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        user.NormalizedUserName = req.Username.ToUpperInvariant();
        user.NormalizedEmail = req.Email.ToUpperInvariant();

        // создаём пользователя через репозиторий
        await _users.AddUserAsync(user, ct);

        // назначаем базовую роль "User"
        var roleId = await GetOrCreateRoleIdAsync("Admin", ct);
        await _users.AddUserRoleAsync(user.Id, roleId, ct);

        // генерим токен с ролями
        var roles = await _users.GetUserRolesAsync(user.Id, ct);
        Console.WriteLine($"[LOGIN] {user.Email} roles: {string.Join(",", roles)}");
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

        // поиск по Username — через DbContext (в интерфейсе нет FindByUsername)
        var norm = req.Username.ToUpperInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == norm, ct);

        if (user is null) return Unauthorized();

        var incomingHash = HashPasswordSha256(req.Password);
        if (!CryptEquals(incomingHash, user.PasswordHash ?? string.Empty))
            return Unauthorized();

        var roles = await _users.GetUserRolesAsync(user.Id, ct);
        var token = _jwt.GenerateToken(user.Id, user.Email!, roles);

        Console.WriteLine($"[LOGIN] user: {user.Email}, id: {user.Id}");
        Console.WriteLine($"[LOGIN] roles: {string.Join(",", roles ?? Array.Empty<string>())}");

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
        // TODO: подключить IRefreshTokenService и валидировать refresh-токен
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
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            "(unknown)";

        var displayName =
            User.Identity?.Name ??
            User.FindFirstValue(ClaimTypes.Name) ??
            User.FindFirstValue(JwtRegisteredClaimNames.Email);

        return Ok(new
        {
            id = userId,
            name = displayName,
            claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }

    // ===== helpers =====

    private async Task<Guid> GetOrCreateRoleIdAsync(string roleName, CancellationToken ct)
    {
        try
        {
            return await _roles.GetRoleIdByNameAsync(roleName, ct);
        }
        catch
        {
            var role = await _roles.CreateAsync(roleName, ct);
            return role.Id;
        }
    }

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
