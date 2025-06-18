namespace IdentityService.Application.DTOs;

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string AccessMode { get; set; } = "AfterDeath";
    public bool DeathConfirmed { get; set; } = false;
    // + другие нужные поля
}
