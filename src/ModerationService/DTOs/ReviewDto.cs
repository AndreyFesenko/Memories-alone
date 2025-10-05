using System;
using ModerationService.Models;

namespace ModerationService.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public string ResourceType { get; set; } = default!;
        public Guid ResourceId { get; set; }
        public string Content { get; set; } = default!;
        public string? MetadataJson { get; set; }
        public ReviewStatus Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DecidedAt { get; set; }
        public string? DecidedBy { get; set; }
        public string? DecisionReason { get; set; }
    }
}
