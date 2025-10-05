using System;

namespace ModerationService.DTOs
{
    public class CreateReviewDto
    {
        public string ResourceType { get; set; } = default!;
        public Guid ResourceId { get; set; }
        public string Content { get; set; } = default!;
        public string? MetadataJson { get; set; }
        public string? CreatedBy { get; set; }
    }
}
