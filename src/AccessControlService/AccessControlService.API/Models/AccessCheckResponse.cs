namespace AccessControlService.API.Models;

public class AccessCheckResponse
{
    public bool HasAccess { get; set; }
    public string Message { get; set; } = "";
}
