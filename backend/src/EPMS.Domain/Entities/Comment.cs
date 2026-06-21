using EPMS.Domain.Common;

namespace EPMS.Domain.Entities;

/// <summary>
/// Komentar yang ditulis user di dalam Task Detail Modal (timeline komentar,
/// terpisah dari ActivityLog yang mencatat aksi sistem otomatis).
/// </summary>
public class Comment : BaseEntity
{
    public Guid TaskId { get; set; }

    public string Content { get; set; } = string.Empty;

    public Guid CreatedByUserId { get; set; }

    // Navigation properties
    public ProjectTask? Task { get; set; }

    public User? CreatedByUser { get; set; }
}
