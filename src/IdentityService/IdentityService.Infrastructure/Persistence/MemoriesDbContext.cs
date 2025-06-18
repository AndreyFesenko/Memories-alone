using Microsoft.EntityFrameworkCore;
using IdentityService.Domain.Entities;

namespace IdentityService.Infrastructure.Persistence;

public class MemoriesDbContext : DbContext
{
    public MemoriesDbContext(DbContextOptions<MemoriesDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");

        // User
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users", "identity");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.UserName).HasColumnName("username");
            b.Property(x => x.NormalizedUserName).HasColumnName("normalizedusername");
            b.Property(x => x.Email).HasColumnName("email");
            b.Property(x => x.NormalizedEmail).HasColumnName("normalizedemail");
            b.Property(x => x.EmailConfirmed).HasColumnName("emailconfirmed");
            b.Property(x => x.PasswordHash).HasColumnName("passwordhash");
            b.Property(x => x.PhoneNumber).HasColumnName("phonenumber");
            b.Property(x => x.PhoneNumberConfirmed).HasColumnName("phonenumberconfirmed");
            b.Property(x => x.TwoFactorEnabled).HasColumnName("twofactorenabled");
            b.Property(x => x.LockoutEnd).HasColumnName("lockoutend");
            b.Property(x => x.LockoutEnabled).HasColumnName("lockoutenabled");
            b.Property(x => x.AccessFailedCount).HasColumnName("accessfailedcount");
            b.Property(x => x.CreatedAt).HasColumnName("createdat");
            b.Property(x => x.LastLoginAt).HasColumnName("lastloginat");
        });

        // Role
        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("roles", "identity");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Name).HasColumnName("name");
            b.Property(x => x.NormalizedName).HasColumnName("normalizedname");
            b.Property(x => x.Description).HasColumnName("description");
            b.HasIndex(x => x.Name).IsUnique();
        });

        // UserRole
        modelBuilder.Entity<UserRole>(b =>
        {
            b.ToTable("userroles", "identity");
            b.HasKey(x => new { x.UserId, x.RoleId });
            b.Property(x => x.UserId).HasColumnName("userid");
            b.Property(x => x.RoleId).HasColumnName("roleid");
            b.HasOne(x => x.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(x => x.UserId);
            b.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId);
        });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("refreshtokens", "identity");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.UserId).HasColumnName("userid");
            b.Property(x => x.Token).HasColumnName("token");
            b.Property(x => x.ExpiresAt).HasColumnName("expiresat");
            b.Property(x => x.CreatedAt).HasColumnName("createdat");
            b.Property(x => x.RevokedAt).HasColumnName("revokedat");
            b.Property(x => x.ReplacedByToken).HasColumnName("replacedbytoken");
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Token);
        });
    }
}
