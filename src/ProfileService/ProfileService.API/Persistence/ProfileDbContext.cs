using Microsoft.EntityFrameworkCore;
using ProfileService.API.Models;

namespace ProfileService.API.Persistence;

public class ProfileDbContext : DbContext
{
    public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options) { }
    public DbSet<ProfileDto> Profiles => Set<ProfileDto>();
}
