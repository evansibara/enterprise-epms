namespace EPMS.Application.DTOs.Tasks;

public class TaskResponseDto
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? AssigneeId { get; set; }

    public string? AssigneeName { get; set; }

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

public class CreateTaskRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? AssigneeId { get; set; }

    public string Priority { get; set; } = "Medium";
}

public class UpdateTaskRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Priority { get; set; } = "Medium";
}

public class UpdateTaskStatusRequestDto
{
    public string Status { get; set; } = string.Empty;
}

public class AssignTaskRequestDto
{
    public Guid AssigneeId { get; set; }
}
