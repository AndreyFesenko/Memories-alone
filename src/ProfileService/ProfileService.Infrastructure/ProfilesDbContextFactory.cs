//C: \Users\user\Source\Repos\Memories-alone\src\ProfileService\ProfileService.Infrastructure\ProfilesDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProfileService.Infrastructure
{
    public class ProfilesDbContextFactory : IDesignTimeDbContextFactory<ProfilesDbContext>
    {
        public ProfilesDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = config.GetConnectionString("Default");

            var optionsBuilder = new DbContextOptionsBuilder<ProfilesDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ProfilesDbContext(optionsBuilder.Options);
        }
    }
}
