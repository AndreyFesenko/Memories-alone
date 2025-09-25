// src/AccessControlService/AccessControlService.API/Controllers/AccessController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AccessControlService.API.Models;
using AccessControlService.Infrastructure.Entities;
using AccessControlService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlService.API.Controllers
{
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
        public async Task<ActionResult<AccessRuleDto>> Grant([FromBody] AccessRuleCreateRequest request, CancellationToken ct)
        {
            var grantedBy = TryGetUserId();

            // Формируем правило заранее с новым Id — это пригодится, чтобы понять "created" или "existing".
            var seedId = Guid.NewGuid();
            var rule = new AccessRule
            {
                Id = seedId,
                SubjectType = request.SubjectType ?? "User",
                SubjectId = request.SubjectId,
                ResourceType = request.ResourceType ?? "Memory",
                ResourceId = request.ResourceId,
                Permission = request.AccessType ?? request.Permission ?? "View",
                ExpiresAt = request.ExpiresAt,
                GrantedBy = grantedBy,
                CreatedAt = DateTime.UtcNow
            };

            // Идемпотентная вставка (вернёт существующую запись, если уникальный ключ уже есть)
            var saved = await _rules.CreateIfNotExistsAsync(rule, ct);
            var created = saved.Id == seedId; // если Id совпал с нашим seedId — это новая вставка

            await _audit.AddAsync(new AuditLog
            {
                Action = created ? "GrantAccess" : "GrantAccessIdempotent",
                Details = $"{(created ? "Granted" : "Exists")} {saved.Permission} to [{saved.SubjectType}:{saved.SubjectId}] for {saved.ResourceType}:{saved.ResourceId}",
                UserId = grantedBy
            }, ct);

            // можно вернуть 201 при создании; для простоты всегда 200
            // return created ? CreatedAtAction(nameof(GetShares), new { id = saved.ResourceId }, ToDto(saved)) : Ok(ToDto(saved));
            return Ok(ToDto(saved));
        }

        /// <summary>Проверить доступ</summary>
        [AllowAnonymous]
        [HttpPost("check")]
        public async Task<ActionResult<AccessCheckResponse>> Check([FromBody] AccessCheckRequest request, CancellationToken ct)
        {
            var has = await _rules.CheckAccessAsync(
                subjectId: request.SubjectId,
                resourceId: request.ResourceId,
                permission: request.AccessType ?? request.Permission ?? "View",
                subjectType: request.SubjectType,
                resourceType: request.ResourceType ?? "Memory",
                ct: ct
            );

            return Ok(new AccessCheckResponse
            {
                HasAccess = has,
                Message = has ? "Access granted" : "Access denied"
            });
        }

        /// <summary>Обновить правило доступа</summary>
        [Authorize(Roles = "admin,access_manager")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AccessRuleUpdateRequest req, CancellationToken ct)
        {
            var rule = await _rules.GetAsync(id, ct);
            if (rule == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(req.SubjectType)) rule.SubjectType = req.SubjectType!.Trim();
            if (!string.IsNullOrWhiteSpace(req.SubjectId)) rule.SubjectId = req.SubjectId!.Trim();
            if (!string.IsNullOrWhiteSpace(req.ResourceType)) rule.ResourceType = req.ResourceType!.Trim();
            if (req.ResourceId.HasValue) rule.ResourceId = req.ResourceId.Value;
            if (!string.IsNullOrWhiteSpace(req.AccessType ?? req.Permission))
                rule.Permission = (req.AccessType ?? req.Permission)!.Trim();
            if (req.ExpiresAt.HasValue) rule.ExpiresAt = req.ExpiresAt;

            await _rules.UpdateAsync(rule, ct);

            await _audit.AddAsync(new AuditLog
            {
                Action = "UpdateAccess",
                Details = $"Updated rule {id}",
                UserId = TryGetUserId()
            }, ct);

            return Ok(ToDto(rule));
        }

        /// <summary>Удалить правило доступа по id</summary>
        [Authorize(Roles = "admin,access_manager")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _rules.DeleteAsync(id, ct);

            await _audit.AddAsync(new AuditLog
            {
                Action = "DeleteAccess",
                Details = $"Deleted rule {id}",
                UserId = TryGetUserId()
            }, ct);

            return NoContent();
        }

        /// <summary>Список правил шаринга для конкретного Memory</summary>
        [Authorize(Roles = "admin,access_manager")]
        [HttpGet("memory/{id:guid}/shares")]
        [ProducesResponseType(typeof(List<AccessRuleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AccessRuleDto>>> GetShares(Guid id, CancellationToken ct)
        {
            var rules = await _rules.GetByResourceAsync(id, ct);
            var dtos = rules.Select(ToDto).ToList();
            return Ok(dtos);
        }

        /// <summary>Отозвать доступ по составному ключу (subject+resource+permission)</summary>
        /// <summary>Отозвать доступ по составному ключу (subject+resource+permission)</summary>
        [Authorize(Roles = "admin,access_manager")]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] AccessRevokeRequest req, CancellationToken ct)
        {
            var subjectType = (req.SubjectType ?? "User").Trim();
            var resourceType = (req.ResourceType ?? "Memory").Trim();
            var permission = (req.Permission ?? "View").Trim();

            var removed = await _rules.DeleteByCompositeAsync(
                subjectId: req.SubjectId.Trim(),
                subjectType: subjectType,
                resourceType: resourceType,
                resourceId: req.ResourceId,
                permission: permission,
                ct: ct
            );

            await _audit.AddAsync(new AuditLog
            {
                Action = "RevokeAccess",
                Details = $"Revoked {permission} from [{subjectType}:{req.SubjectId}] for {resourceType}:{req.ResourceId}. Removed={removed}",
                UserId = TryGetUserId()
            }, ct);

            // Возвращаем JSON вместо 204 NoContent
            return Ok(new
            {
                removed,
                subjectId = req.SubjectId,
                subjectType,
                resourceType,
                resourceId = req.ResourceId,
                permission
            });
        }

        private Guid? TryGetUserId()
        {
            try
            {
                var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue("sub")
                          ?? User.Identity?.Name;

                return Guid.TryParse(sub, out var g) ? g : null;
            }
            catch { return null; }
        }

        private static AccessRuleDto ToDto(AccessRule r) => new()
        {
            Id = r.Id,
            SubjectType = r.SubjectType,
            SubjectId = r.SubjectId,
            ResourceType = r.ResourceType,
            ResourceId = r.ResourceId,
            Permission = r.Permission,
            ExpiresAt = r.ExpiresAt,
            GrantedBy = r.GrantedBy,
            CreatedAt = r.CreatedAt
        };
    }
}
