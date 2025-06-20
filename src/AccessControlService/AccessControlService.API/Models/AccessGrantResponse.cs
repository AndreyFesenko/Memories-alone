namespace AccessControlService.API.Models;

public class AccessGrantResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = default!;
}
