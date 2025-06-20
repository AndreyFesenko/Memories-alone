namespace ProfileService.API.Models;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public DateTime? BirthDate { get; set; }
    public bool DeathConfirmed { get; set; }
    public string? AccessMode { get; set; } // Например: "Anytime" | "AfterDeath"
}
