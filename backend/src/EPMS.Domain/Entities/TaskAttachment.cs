using EPMS.Domain.Common;

namespace EPMS.Domain.Entities;

public class TaskAttachment : BaseEntity
{
    public Guid TaskId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    // Navigation property
    public ProjectTask? Task { get; set; }
}
