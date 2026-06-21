using EPMS.Domain.Common;
using EPMS.Domain.Enums;

namespace EPMS.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

    public Guid OwnerId { get; set; }

    // Navigation properties
    public User? Owner { get; set; }

    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}
