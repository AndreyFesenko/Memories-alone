using AccessControlService.API.Models;
using AccessControlService.Domain.Entities;
using AccessControlService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using System.Security.AccessControl;

namespace AccessControlService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccessController : ControllerBase
{
    private readonly AccessRuleRepository _rules;
    private readonly AuditLogRepository _audit;

    public AccessController(AccessRuleRepository rules, AuditLogRepository audit)
    {
        _rules = rules;
        _audit = audit;
    }

    /// <summary>Создать правило доступа (Grant)</summary>
    [Authorize(Roles = "admin,access_manager")]
    [HttpPost("grant")]
    public async Task<ActionResult<AccessRuleDto>> Grant([FromBody] AccessRuleCreateRequest request)
    {
        var rule = new AccessRule
        {
            Id = Guid.NewGuid(),
            SubjectId = request.SubjectId,
            ObjectId = request.ObjectId,
            AccessType = request.AccessType,
            GrantedBy = request.GrantedBy,
            CreatedAt = DateTime.UtcNow
        };
        await _rules.CreateAsync(rule);
        await _audit.AddAsync(new AuditLog
        {
            Action = "GrantAccess",
            Details = $"Granted {request.AccessType} to {request.SubjectId} for {request.ObjectId}",
            UserId = User.Identity?.IsAuthenticated == true ? Guid.Parse(User.Identity.Name!) : null
        });
        return Ok(ToDto(rule));
    }

    /// <summary>Проверить доступ</summary>
    [AllowAnonymous]
    [HttpPost("check")]
    public async Task<ActionResult<AccessCheckResponse>> Check([FromBody] AccessCheckRequest request)
    {
        var has = await _rules.CheckAccessAsync(request.SubjectId, request.ObjectId, request.AccessType);
        return Ok(new AccessCheckResponse
        {
            HasAccess = has,
            Message = has ? "Access granted" : "Access denied"
        });
    }

    /// <summary>Обновить правило доступа</summary>
    [Authorize(Roles = "admin,access_manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AccessRuleUpdateRequest req)
    {
        var rule = await _rules.GetAsync(id);
        if (rule == null) return NotFound();
        if (!string.IsNullOrEmpty(req.AccessType)) rule.AccessType = req.AccessType;
        await _rules.UpdateAsync(rule);

        await _audit.AddAsync(new AuditLog
        {
            Action = "UpdateAccess",
            Details = $"Updated rule {id}",
            UserId = User.Identity?.IsAuthenticated == true ? Guid.Parse(User.Identity.Name!) : null
        });
        return Ok(ToDto(rule));
    }

    /// <summary>Удалить правило доступа</summary>
    [Authorize(Roles = "admin,access_manager")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _rules.DeleteAsync(id);

        await _audit.AddAsync(new AuditLog
        {
            Action = "DeleteAccess",
            Details = $"Deleted rule {id}",
            UserId = User.Identity?.IsAuthenticated == true ? Guid.Parse(User.Identity.Name!) : null
        });
        return NoContent();
    }

    /// <summary>Получить все правила</summary>
    [Authorize(Roles = "admin,access_manager")]
    [HttpGet]
    public async Task<ActionResult<List<AccessRuleDto>>> GetAll()
    {
        var all = await _rules.GetAllAsync();
        return Ok(all.Select(ToDto).ToList());
    }

    private static AccessRuleDto ToDto(AccessRule r) => new()
    {
        Id = r.Id,
        SubjectId = r.SubjectId,
        ObjectId = r.ObjectId,
        AccessType = r.AccessType,
        GrantedBy = r.GrantedBy,
        CreatedAt = r.CreatedAt
    };
}
