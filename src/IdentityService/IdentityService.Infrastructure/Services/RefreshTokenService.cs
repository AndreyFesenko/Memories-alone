using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly MemoriesDbContext _db;
    private readonly IJwtTokenGenerator _jwt;

    public RefreshTokenService(MemoriesDbContext db, IJwtTokenGenerator jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<RefreshTokenResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenEntity = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedAt == null, cancellationToken);

        if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token is invalid or expired");

        var user = await _db.Users.FindAsync(tokenEntity.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Генерируем новый токен доступа и refresh
        var newRefreshToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

        // Получи роли пользователя!
        var roles = await _db.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        var accessToken = _jwt.GenerateToken(user.Id, user.Email, roles);

        // Помечаем старый refresh как отозванный
        tokenEntity.RevokedAt = DateTime.UtcNow;

        // Сохраняем новый refresh в базу
        _db.RefreshTokens.Add(new IdentityService.Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(14),
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(accessToken, newRefreshToken);
    }

    public async Task InvalidateAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var entity = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);
        if (entity != null && entity.RevokedAt == null)
        {
            entity.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RevokeAsync(Guid userId, string refreshToken, CancellationToken ct)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshToken, ct);
        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

}
