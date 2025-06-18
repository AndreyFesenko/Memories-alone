using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using IdentityService.Application.DTOs;
using IdentityService.Application.Commands;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/admin/roles")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RolesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<List<RoleDto>> GetRoles() => await _mediator.Send(new GetRolesQuery());

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleCommand cmd)
        => Ok(await _mediator.Send(cmd));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        await _mediator.Send(new DeleteRoleCommand { RoleId = id });
        return NoContent();
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(AssignRoleCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok();
    }
    
    [HttpGet("{userId}/roles")]
    public async Task<ActionResult<List<string>>> GetUserRoles(Guid userId)
    => Ok(await _mediator.Send(new GetUserRolesQuery { UserId = userId }));
    
    [HttpPost("unassign")]
    public async Task<IActionResult> Unassign([FromBody] UnassignRoleCommand cmd)
    => Ok(await _mediator.Send(cmd));

    [HttpGet("by-role/{roleId}")]
    public async Task<ActionResult<List<Guid>>> GetUsersByRole(Guid roleId)
    => Ok(await _mediator.Send(new GetUsersByRoleQuery { RoleId = roleId }));
}
