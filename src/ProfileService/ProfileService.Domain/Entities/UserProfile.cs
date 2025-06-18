namespace ProfileService.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string AccessMode { get; set; } = ""AfterDeath""; // РёР»Рё ""Anytime""
    public string? Biography { get; set; }
    public bool DeathConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
