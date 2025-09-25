// src\AccessControlService\AccessControlService.Infrastructure\AccessDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AccessControlService.Infrastructure
{
    public class AccessDbContextFactory : IDesignTimeDbContextFactory<AccessDbContext>
    {
        public AccessDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AccessDbContext>();
            var connectionString = config.GetConnectionString("Default");
            optionsBuilder.UseNpgsql(connectionString);

            return new AccessDbContext(optionsBuilder.Options);
        }
    }
}


