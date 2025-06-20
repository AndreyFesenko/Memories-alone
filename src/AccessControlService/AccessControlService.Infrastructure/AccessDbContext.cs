using AccessControlService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AccessControlService.Infrastructure;

public class AccessDbContext : DbContext
{
    public AccessDbContext(DbContextOptions<AccessDbContext> options)
    : base(options)
    {
    }

    public DbSet<AccessRule> AccessRules => Set<AccessRule>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
}
