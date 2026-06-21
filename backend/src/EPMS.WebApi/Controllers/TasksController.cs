using EPMS.Application.Common;
using EPMS.Application.DTOs.Tasks;
using EPMS.Application.Interfaces.Services;
using EPMS.Application.UseCases.Tasks;
using EPMS.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.WebApi.Controllers;

[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ICurrentUserService _currentUserService;

    public TasksController(ITaskService taskService, ICurrentUserService currentUserService)
    {
        _taskService = taskService;
        _currentUserService = currentUserService;
    }

    /// <summary>GET /api/v1/projects/{projectId}/tasks — list semua task dalam satu project.</summary>
    [HttpGet("api/v1/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> ListByProject(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _taskService.ListByProjectAsync(projectId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TaskResponseDto>>.Ok(result));
    }

    [HttpGet("api/v1/tasks/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _taskService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<TaskResponseDto>.Ok(result));
    }

    /// <summary>POST /api/v1/projects/{projectId}/tasks — buat task baru dalam project.</summary>
    [HttpPost("api/v1/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskRequestDto request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserIdOrThrow();
        var result = await _taskService.CreateAsync(projectId, userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<TaskResponseDto>.Ok(result, "Task berhasil dibuat."));
    }

    [HttpPut("api/v1/tasks/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequestDto request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserIdOrThrow();
        var result = await _taskService.UpdateAsync(id, userId, request, cancellationToken);
        return Ok(ApiResponse<TaskResponseDto>.Ok(result, "Task berhasil diperbarui."));
    }

    /// <summary>PATCH /api/v1/tasks/{id}/status — dipanggil saat drag-drop kolom Kanban di frontend.</summary>
    [HttpPatch("api/v1/tasks/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusRequestDto request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserIdOrThrow();
        var result = await _taskService.UpdateStatusAsync(id, userId, request, cancellationToken);
        return Ok(ApiResponse<TaskResponseDto>.Ok(result, "Status task berhasil diperbarui."));
    }

    [HttpPatch("api/v1/tasks/{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, AssignTaskRequestDto request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserIdOrThrow();
        var result = await _taskService.AssignAsync(id, userId, request, cancellationToken);
        return Ok(ApiResponse<TaskResponseDto>.Ok(result, "Task berhasil ditugaskan."));
    }

    [HttpDelete("api/v1/tasks/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = CurrentUserIdOrThrow();
        await _taskService.DeleteAsync(id, userId, cancellationToken);
        return Ok(ApiResponse.Ok("Task berhasil dihapus."));
    }

    private Guid CurrentUserIdOrThrow() =>
        _currentUserService.UserId ?? throw new UnauthorizedException("Sesi tidak valid.");
}
