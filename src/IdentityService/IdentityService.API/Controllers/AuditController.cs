using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityService.Infrastructure.Persistence;

namespace IdentityService.API.Controllers
{
    [Authorize(Roles="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly MemoriesDbContext _db;
        public AuditController(MemoriesDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int limit = 100)
            => Ok(await _db.AuditLogs.OrderByDescending(x => x.CreatedAt).Take(limit).ToListAsync());
    }
}
