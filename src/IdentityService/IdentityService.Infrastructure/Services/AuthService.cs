using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace IdentityService.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _jwt;
    private readonly MemoriesDbContext _db;

    public AuthService(IUserRepository users, IJwtTokenGenerator jwt, MemoriesDbContext db)
    {
        _users = users;
        _jwt = jwt;
        _db = db;
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var user = await _users.FindByEmailAsync(email, CancellationToken.None);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var roles = await _users.GetUserRolesAsync(user.Id, CancellationToken.None);
        var token = _jwt.GenerateToken(user.Id, user.Email, roles);
        var refreshToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(14),
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return new LoginResponse(token, refreshToken);
    }
}
