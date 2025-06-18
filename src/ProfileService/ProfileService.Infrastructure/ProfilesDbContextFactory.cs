using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProfileService.Infrastructure.Persistence;

namespace ProfileService.Infrastructure;

public class ProfilesDbContextFactory : IDesignTimeDbContextFactory<ProfilesDbContext>
{
    public ProfilesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProfilesDbContext>();
        optionsBuilder.UseNpgsql(""Host=localhost;Port=5432;Database=ProfileServiceDb;Username=postgres;Password=postgres"");

        return new ProfilesDbContext(optionsBuilder.Options);
    }
}
