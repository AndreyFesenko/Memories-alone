namespace ProfileService.Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = default!;
    public DateTime? BirthDate { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public bool DeathConfirmed { get; set; }
    // "Anytime" | "AfterDeath"
    public string AccessMode { get; set; } = "AfterDeath";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? AvatarUrl { get; set; }
}
