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
