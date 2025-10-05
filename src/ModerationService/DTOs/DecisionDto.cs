namespace ModerationService.DTOs
{
    public class DecisionDto
    {
        public string Decision { get; set; } = default!;
        public string? ModeratorId { get; set; }
        public string? Reason { get; set; }
    }
}
