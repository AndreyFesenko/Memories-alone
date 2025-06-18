using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IdentityService.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MemoriesDbContext>
    {
        public MemoriesDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<MemoriesDbContext>();
            var connectionString = configuration.GetConnectionString("Default");

            builder.UseNpgsql(connectionString);

            return new MemoriesDbContext(builder.Options);
        }
    }
}
