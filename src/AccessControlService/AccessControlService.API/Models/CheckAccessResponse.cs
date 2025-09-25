namespace AccessControlService.API.Models;

public sealed class CheckAccessResponse
{
    public bool HasAccess { get; set; }
    public string? Message { get; set; }
}
