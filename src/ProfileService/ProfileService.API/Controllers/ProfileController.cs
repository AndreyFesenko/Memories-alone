using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Application.Commands;
using ProfileService.Application.DTOs;
using ProfileService.Application.Queries;

namespace ProfileService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/profile/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfileDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetProfileByIdQuery { Id = id }, ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    // GET /api/profile/by-user/{userId}
    [HttpGet("by-user/{userId:guid}")]
    public async Task<ActionResult<UserProfileDto>> GetByUserId(Guid userId, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetProfileByUserIdQuery { UserId = userId }, ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    // POST /api/profile
    [HttpPost]
    [Authorize] // при желании можно снять, если публично
    public async Task<ActionResult<UserProfileDto>> Create(CreateProfileCommand cmd, CancellationToken ct)
    {
        var dto = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    // PUT /api/profile/{id}
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> Update(Guid id, UpdateProfileCommand cmd, CancellationToken ct)
    {
        if (id != cmd.Id) return BadRequest("Route id != body id");
        var dto = await _mediator.Send(cmd, ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    // DELETE /api/profile/{id}
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProfileCommand { Id = id }, ct);
        return NoContent();
    }

    // POST /api/profile/{userId}/confirm-death
    [HttpPost("{userId:guid}/confirm-death")]
    [Authorize]
    public async Task<IActionResult> ConfirmDeath(Guid userId, CancellationToken ct)
    {
        await _mediator.Send(new ConfirmDeathCommand { UserId = userId }, ct);
        return Ok(new { message = "Death confirmed" });
    }
}
