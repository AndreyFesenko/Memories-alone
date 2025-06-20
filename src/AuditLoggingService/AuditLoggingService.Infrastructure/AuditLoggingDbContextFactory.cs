// Infrastructure/Persistence/AuditLoggingDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuditLoggingService.Infrastructure.Persistence;

public class AuditLoggingDbContextFactory : IDesignTimeDbContextFactory<AuditLoggingDbContext>
{
    public AuditLoggingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditLoggingDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=AuditLoggingDb;Username=postgres;Password=admin");
        return new AuditLoggingDbContext(optionsBuilder.Options);
    }
}
