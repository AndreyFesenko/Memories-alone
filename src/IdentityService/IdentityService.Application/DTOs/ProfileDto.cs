namespace IdentityService.Application.DTOs
{
    public class ProfileDto
    {
        public Guid UserId { get; set; }
        public string AccessMode { get; set; } = "AfterDeath";
        public bool DeathConfirmed { get; set; } = false;
    }
}
