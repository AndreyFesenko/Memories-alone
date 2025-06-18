using Microsoft.EntityFrameworkCore;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure.Persistence;

public class ProfilesDbContext : DbContext
{
    public ProfilesDbContext(DbContextOptions<ProfilesDbContext> options)
        : base(options) { }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.UserId).IsUnique();
            b.Property(x => x.AccessMode).HasDefaultValue(""AfterDeath"");
            b.Property(x => x.CreatedAt).HasDefaultValueSql(""CURRENT_TIMESTAMP"");
            b.Property(x => x.UpdatedAt).HasDefaultValueSql(""CURRENT_TIMESTAMP"");
        });
    }
}
