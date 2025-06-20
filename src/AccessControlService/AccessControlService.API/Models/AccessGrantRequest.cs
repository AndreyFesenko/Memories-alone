namespace AccessControlService.API.Models;

public class AccessGrantRequest
{
    public Guid SubjectId { get; set; }    // Кому даём доступ (user)
    public Guid ObjectId { get; set; }     // К чему даём доступ (ресурс)
    public string AccessType { get; set; } = "read"; // Тип доступа (read/write/admin/...)
    // public DateTime? ExpiresAt { get; set; }
}
