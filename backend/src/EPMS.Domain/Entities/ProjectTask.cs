using EPMS.Domain.Common;
using EPMS.Domain.Enums;
using TaskStatusEnum = EPMS.Domain.Enums.TaskStatus;

namespace EPMS.Domain.Entities;

public class ProjectTask : BaseEntity
{
    public Guid ProjectId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? AssigneeId { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.ToDo;

    // Navigation properties
    public Project? Project { get; set; }

    public User? Assignee { get; set; }

    public ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
}
