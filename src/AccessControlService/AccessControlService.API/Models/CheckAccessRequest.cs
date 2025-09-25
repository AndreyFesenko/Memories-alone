using System;

namespace AccessControlService.API.Models;

public sealed class CheckAccessRequest
{
    public SubjectType SubjectType { get; set; }
    public string SubjectId { get; set; } = "*";
    public string ResourceType { get; set; } = "Memory";
    public Guid ResourceId { get; set; }
    public string Permission { get; set; } = "View";
}
