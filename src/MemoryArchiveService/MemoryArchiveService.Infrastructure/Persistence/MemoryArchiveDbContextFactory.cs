using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace MemoryArchiveService.Infrastructure.Persistence;

public class MemoryArchiveDbContextFactory : IDesignTimeDbContextFactory<MemoryArchiveDbContext>
{
    public MemoryArchiveDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MemoryArchiveDbContext>();
        var connectionString = config.GetConnectionString("Default");

        optionsBuilder.UseNpgsql(connectionString);

        return new MemoryArchiveDbContext(optionsBuilder.Options);
    }
}
