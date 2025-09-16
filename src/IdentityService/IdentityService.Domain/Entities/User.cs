namespace IdentityService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string NormalizedUserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = null!;
	public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmedAt { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetRequestedAt { get; set; }

}