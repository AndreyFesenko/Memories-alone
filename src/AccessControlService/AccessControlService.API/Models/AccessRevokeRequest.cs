namespace AccessControlService.API.Models
{
    public class AccessRevokeRequest
    {
        public string SubjectId { get; set; } = default!;
        public string? SubjectType { get; set; } // "User" | "Role"
        public string? ResourceType { get; set; } // "Memory"
        public Guid ResourceId { get; set; }
        public string? Permission { get; set; } // "View" | "Edit" | "Delete" | "Read"
    }
}
