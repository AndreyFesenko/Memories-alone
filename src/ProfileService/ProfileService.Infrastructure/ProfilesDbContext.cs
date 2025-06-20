using Microsoft.EntityFrameworkCore;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure;

public class ProfilesDbContext : DbContext
{
    public ProfilesDbContext(DbContextOptions<ProfilesDbContext> options)
        : base(options) { }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("profile");
        modelBuilder.Entity<UserProfile>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.DisplayName).HasMaxLength(100);
            b.Property(x => x.Bio).HasMaxLength(500);
            b.Property(x => x.AccessMode).HasDefaultValue("AfterDeath");
        });
    }
}
