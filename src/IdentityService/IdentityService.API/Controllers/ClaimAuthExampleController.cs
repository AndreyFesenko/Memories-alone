using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClaimAuthExampleController : ControllerBase
{
    // Доступен только если claim "DeathConfirmed" == "true"
    [Authorize(Policy = "DeathConfirmedOnly")]
    [HttpGet("death-confirmed")]
    public IActionResult OnlyForDeathConfirmed()
    {
        return Ok("У тебя подтверждён статус DeathConfirmed!");
    }

    // Доступен только если claim AccessMode == "Anytime"
    [Authorize(Policy = "AccessAnytimeOnly")]
    [HttpGet("anytime")]
    public IActionResult OnlyForAnytimeAccess()
    {
        return Ok("У тебя доступ 'Anytime'!");
    }

    // Универсальный доступ по роли и claim
    [Authorize(Roles = "AccountHolder,CloseRelative", Policy = "DeathConfirmedOnly")]
    [HttpGet("role-and-claim")]
    public IActionResult RoleAndClaim()
    {
        return Ok("Ты владелец или близкий, и подтверждён!");
    }

    // Получить значения claims
    [Authorize]
    [HttpGet("my-claims")]
    public IActionResult MyClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
