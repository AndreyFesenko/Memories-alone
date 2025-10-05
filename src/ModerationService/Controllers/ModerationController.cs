using Microsoft.AspNetCore.Mvc;
using ModerationService.DTOs;
using ModerationService.Models;
using ModerationService.Repositories;

namespace ModerationService.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ModerationController : ControllerBase
    {
        private readonly IModerationRepository _repo;
        private readonly ILogger<ModerationController> _logger;

        public ModerationController(IModerationRepository repo, ILogger<ModerationController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ResourceType) || dto.ResourceId == Guid.Empty || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("ResourceType, ResourceId and Content are required.");

            var model = new ModerationReview
            {
                ResourceType = dto.ResourceType,
                ResourceId = dto.ResourceId,
                Content = dto.Content,
                MetadataJson = dto.MetadataJson,
                CreatedBy = dto.CreatedBy
            };

            await _repo.CreateAsync(model);

            var result = new ReviewDto
            {
                Id = model.Id,
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
                Content = model.Content,
                MetadataJson = model.MetadataJson,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedAt = model.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null) return NotFound();

            var dto = new ReviewDto
            {
                Id = model.Id,
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
                Content = model.Content,
                MetadataJson = model.MetadataJson,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedAt = model.CreatedAt,
                DecidedAt = model.DecidedAt,
                DecidedBy = model.DecidedBy,
                DecisionReason = model.DecisionReason
            };

            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var items = await _repo.ListAsync(status, page, pageSize);
            var dtos = items.Select(m => new ReviewDto
            {
                Id = m.Id,
                ResourceType = m.ResourceType,
                ResourceId = m.ResourceId,
                Content = m.Content,
                MetadataJson = m.MetadataJson,
                Status = m.Status,
                CreatedBy = m.CreatedBy,
                CreatedAt = m.CreatedAt,
                DecidedAt = m.DecidedAt,
                DecidedBy = m.DecidedBy,
                DecisionReason = m.DecisionReason
            });
            return Ok(new { items = dtos, total = dtos.Count() });
        }

        [HttpPost("{id:guid}/decide")]
        public async Task<IActionResult> Decide([FromRoute] Guid id, [FromBody] DecisionDto dto)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null) return NotFound();

            if (!Enum.TryParse<ReviewStatus>(dto.Decision, true, out var decision))
                return BadRequest("Decision must be 'Approved' or 'Rejected'.");

            model.Status = decision;
            model.DecidedAt = DateTime.UtcNow;
            model.DecidedBy = dto.ModeratorId;
            model.DecisionReason = dto.Reason;

            await _repo.UpdateAsync(model);

            var res = new ReviewDto
            {
                Id = model.Id,
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
                Content = model.Content,
                MetadataJson = model.MetadataJson,
                Status = model.Status,
                CreatedBy = model.CreatedBy,
                CreatedAt = model.CreatedAt,
                DecidedAt = model.DecidedAt,
                DecidedBy = model.DecidedBy,
                DecisionReason = model.DecisionReason
            };

            return Ok(res);
        }
    }
}
