namespace EPMS.Application.DTOs.ActivityLogs;

public class ActivityLogResponseDto
{
    public Guid Id { get; set; }

    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public Guid PerformedByUserId { get; set; }

    public string? PerformedByUserName { get; set; }

    public DateTime Timestamp { get; set; }

    public string? MetadataJson { get; set; }
}
