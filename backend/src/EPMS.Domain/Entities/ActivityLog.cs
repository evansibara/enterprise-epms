using EPMS.Domain.Common;

namespace EPMS.Domain.Entities;

/// <summary>
/// Mencatat aktivitas/perubahan pada entity lain (Project, ProjectTask, dll)
/// untuk keperluan audit trail. Direkam otomatis tiap ada perubahan (lihat
/// use case RecordActivity di Application layer).
/// </summary>
public class ActivityLog : BaseEntity
{
    /// <summary>Nama tipe entity yang terkait, misal "Project" atau "ProjectTask".</summary>
    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    /// <summary>Aksi yang dilakukan, misal "Created", "StatusChanged", "Deleted".</summary>
    public string Action { get; set; } = string.Empty;

    public Guid PerformedByUserId { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>Data tambahan dalam bentuk JSON string, misal { "from": "ToDo", "to": "Done" }.</summary>
    public string? MetadataJson { get; set; }

    // Navigation property
    public User? PerformedByUser { get; set; }
}
