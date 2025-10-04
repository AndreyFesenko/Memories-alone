using Microsoft.EntityFrameworkCore;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure;

public class ProfilesDbContext : DbContext
{
    public ProfilesDbContext(DbContextOptions<ProfilesDbContext> options) : base(options) { }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("profile");

        modelBuilder.Entity<UserProfile>(b =>
        {
            b.ToTable("UserProfiles");
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.UserId).IsUnique(); // у профиля 1:1 с пользователем

            b.Property(x => x.FullName).IsRequired();
            b.Property(x => x.DisplayName).HasMaxLength(100);
            b.Property(x => x.Bio).HasMaxLength(1000);
            b.Property(x => x.AccessMode).HasMaxLength(20).HasDefaultValue("AfterDeath");
            b.Property(x => x.CreatedAt).IsRequired();
        });
    }
}
