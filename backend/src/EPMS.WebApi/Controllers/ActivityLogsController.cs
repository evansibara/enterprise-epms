using EPMS.Application.Common;
using EPMS.Application.DTOs.ActivityLogs;
using EPMS.Application.UseCases.ActivityLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class ActivityLogsController : ControllerBase
{
    private readonly IActivityLogQueryService _activityLogQueryService;

    public ActivityLogsController(IActivityLogQueryService activityLogQueryService)
    {
        _activityLogQueryService = activityLogQueryService;
    }

    /// <summary>GET /api/v1/projects/{projectId}/activity-logs</summary>
    [HttpGet("projects/{projectId:guid}/activity-logs")]
    public async Task<IActionResult> ListByProject(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _activityLogQueryService.ListByEntityAsync("Project", projectId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ActivityLogResponseDto>>.Ok(result));
    }

    /// <summary>GET /api/v1/tasks/{taskId}/activity-logs</summary>
    [HttpGet("tasks/{taskId:guid}/activity-logs")]
    public async Task<IActionResult> ListByTask(Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _activityLogQueryService.ListByEntityAsync("ProjectTask", taskId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ActivityLogResponseDto>>.Ok(result));
    }
}
