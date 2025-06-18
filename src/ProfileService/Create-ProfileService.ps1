# ---------------------------------------------
# Создаёт минимальный ProfileService для MVP
# ---------------------------------------------

$ErrorActionPreference = "Stop"

# 1. Создать решение и проекты
dotnet new sln -n ProfileService
dotnet new webapi -n ProfileService.API
dotnet new classlib -n ProfileService.Application
dotnet new classlib -n ProfileService.Domain
dotnet new classlib -n ProfileService.Infrastructure

# 2. Добавить проекты в решение и ссылки
dotnet sln ProfileService.sln add ProfileService.API ProfileService.Application ProfileService.Domain ProfileService.Infrastructure

dotnet add ProfileService.API reference ProfileService.Domain ProfileService.Infrastructure
dotnet add ProfileService.Infrastructure reference ProfileService.Domain

# 3. Установить NuGet-пакеты
dotnet add ProfileService.API package Swashbuckle.AspNetCore
dotnet add ProfileService.API package Microsoft.EntityFrameworkCore.Design
dotnet add ProfileService.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL

# 4. Создать необходимые папки
New-Item -ItemType Directory -Path ProfileService.Domain\Entities -Force
New-Item -ItemType Directory -Path ProfileService.API\Controllers -Force
New-Item -ItemType Directory -Path ProfileService.Infrastructure\Persistence -Force

# 5. UserProfile.cs
@"
namespace ProfileService.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string AccessMode { get; set; } = ""AfterDeath""; // или ""Anytime""
    public string? Biography { get; set; }
    public bool DeathConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
"@ | Set-Content -Encoding UTF8 ProfileService.Domain\Entities\UserProfile.cs

# 6. ProfilesDbContext.cs
@"
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
"@ | Set-Content -Encoding UTF8 ProfileService.Infrastructure\Persistence\ProfilesDbContext.cs

# 7. ProfilesDbContextFactory.cs (для миграций)
@"
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
"@ | Set-Content -Encoding UTF8 ProfileService.Infrastructure\ProfilesDbContextFactory.cs

# 8. ProfileController.cs
@"
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileService.Domain.Entities;
using ProfileService.Infrastructure.Persistence;

namespace ProfileService.API.Controllers;

[ApiController]
[Route(""api/[controller]"")]
public class ProfileController : ControllerBase
{
    private readonly ProfilesDbContext _db;

    public ProfileController(ProfilesDbContext db)
    {
        _db = db;
    }

    [HttpGet(""{userId:guid}"")]
    public async Task<IActionResult> GetProfile(Guid userId)
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] UserProfile profile)
    {
        profile.Id = Guid.NewGuid();
        profile.CreatedAt = DateTime.UtcNow;
        profile.UpdatedAt = DateTime.UtcNow;
        _db.UserProfiles.Add(profile);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProfile), new { userId = profile.UserId }, profile);
    }

    [HttpPut(""{userId:guid}"")]
    public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UserProfile update)
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        if (profile == null) return NotFound();
        profile.FirstName = update.FirstName;
        profile.LastName = update.LastName;
        profile.BirthDate = update.BirthDate;
        profile.DeathDate = update.DeathDate;
        profile.AccessMode = update.AccessMode;
        profile.Biography = update.Biography;
        profile.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(profile);
    }

    [HttpPost(""{userId:guid}/confirm-death"")]
    public async Task<IActionResult> ConfirmDeath(Guid userId)
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        if (profile == null) return NotFound();
        profile.DeathConfirmed = true;
        profile.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(profile);
    }
}
"@ | Set-Content -Encoding UTF8 ProfileService.API\Controllers\ProfileController.cs

# 9. appsettings.Development.json
@"
{
  ""ConnectionStrings"": {
    ""Default"": ""Host=localhost;Port=5432;Database=ProfileServiceDb;Username=postgres;Password=postgres""
  },
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }
  }
}
"@ | Set-Content -Encoding UTF8 ProfileService.API\appsettings.Development.json

# 10. Program.cs
@"
using Microsoft.EntityFrameworkCore;
using ProfileService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProfilesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(""Default"")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
"@ | Set-Content -Encoding UTF8 ProfileService.API\Program.cs

Write-Host "`n✅ ProfileService project created! Next steps:"
Write-Host "1. Check connection string in ProfileService.API\appsettings.Development.json"
Write-Host "2. In terminal:"
Write-Host "   dotnet restore"
Write-Host "   dotnet build"
Write-Host "   dotnet ef migrations add InitialCreate -p ProfileService.Infrastructure -s ProfileService.API -c ProfilesDbContext -o Persistence/Migrations"
Write-Host "   dotnet ef database update -p ProfileService.Infrastructure -s ProfileService.API -c ProfilesDbContext"
Write-Host "   dotnet run --project ProfileService.API"
Write-Host "`nOpen http://localhost:5000/swagger to test!"
