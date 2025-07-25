using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuditLoggingService.Infrastructure.Persistence;

public class AuditLoggingDbContextFactory : IDesignTimeDbContextFactory<AuditLoggingDbContext>
{
    public AuditLoggingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditLoggingDbContext>();
        optionsBuilder.UseNpgsql("Host=aws-0-ap-southeast-1.pooler.supabase.com;Database=postgres;Username=postgres.znrtufwemqgynxxvjeox;Password=Memories2025!;SSL Mode=Require;Trust Server Certificate=true");

        return new AuditLoggingDbContext(optionsBuilder.Options);
    }
}
