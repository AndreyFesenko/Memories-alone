using System;

namespace ModerationService.DTOs
{
    public class DecisionDto
    {
        public string Decision { get; set; } = default!; // "Approved" or "Rejected"
        public string? ModeratorId { get; set; }
        public string? Reason { get; set; }
    }
}
