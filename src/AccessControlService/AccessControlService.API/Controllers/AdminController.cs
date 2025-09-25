using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccessControlService.API.Models;
using AccessControlService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControlService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AuditLogRepository _audit;

        public AdminController(AuditLogRepository audit)
        {
            _audit = audit;
        }

        /// <summary>Получить последние записи аудита</summary>
        [Authorize(Roles = "admin,access_manager")]
        [HttpGet("audit-logs")]
        public async Task<ActionResult<List<AuditLogDto>>> GetAuditLogs()
        {
            var logs = await _audit.GetAllAsync();
            var result = logs.Select(x => new AuditLogDto
            {
                Id = x.Id,
                Action = x.Action,
                Details = x.Details,
                UserId = x.UserId,
                CreatedAt = x.CreatedAt
            }).ToList();

            return Ok(result);
        }
    }
}
