using System;

namespace AccessControlService.API.Models;

public sealed class GrantAccessRequest
{
    public SubjectType SubjectType { get; set; }
    public string SubjectId { get; set; } = "*"; // "*" для Public/Auth
    public string ResourceType { get; set; } = "Memory";
    public Guid ResourceId { get; set; }
    public string Permission { get; set; } = "View";
    public DateTimeOffset? ExpiresAt { get; set; }
    public string? GrantedBy { get; set; }
}
