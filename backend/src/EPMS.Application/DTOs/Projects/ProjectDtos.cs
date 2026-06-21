namespace EPMS.Application.DTOs.Projects;

public class ProjectResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public string Status { get; set; } = string.Empty;

    public Guid OwnerId { get; set; }

    public string? OwnerName { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateProjectRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }
}

public class UpdateProjectRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public string Status { get; set; } = string.Empty;
}

public class ListProjectsQueryDto
{
    public string? Search { get; set; }

    public string? Status { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
