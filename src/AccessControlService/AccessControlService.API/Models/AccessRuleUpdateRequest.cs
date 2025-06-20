namespace AccessControlService.API.Models;

public class AccessRuleUpdateRequest
{
    public Guid Id { get; set; }
    public string? AccessType { get; set; }
}
