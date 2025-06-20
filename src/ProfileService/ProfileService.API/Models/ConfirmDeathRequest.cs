namespace ProfileService.API.Models;

public class ConfirmDeathRequest
{
    public Guid ProfileId { get; set; }
    public string Reason { get; set; } = default!;
}
