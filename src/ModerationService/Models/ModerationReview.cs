using System;

namespace ModerationService.Models
{
    public enum ReviewStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class ModerationReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ResourceType { get; set; } = default!;
        public Guid ResourceId { get; set; }
        public string Content { get; set; } = default!;
        public string? MetadataJson { get; set; }
        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DecidedAt { get; set; }
        public string? DecidedBy { get; set; }
        public string? DecisionReason { get; set; }
    }
}
