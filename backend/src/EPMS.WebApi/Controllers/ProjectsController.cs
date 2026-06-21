using EPMS.Application.Common;
using EPMS.Application.DTOs.Common;
using EPMS.Application.DTOs.Projects;
using EPMS.Application.Interfaces.Services;
using EPMS.Application.UseCases.Projects;
using EPMS.Domain.Exceptions;
using EPMS.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.WebApi.Controllers;

[ApiController]
[Route("api/v1/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ICurrentUserService _currentUserService;

    public ProjectsController(IProjectService projectService, ICurrentUserService currentUserService)
    {
        _projectService = projectService;
        _currentUserService = currentUserService;
    }

    /// <summary>GET /api/v1/projects?search=&amp;status=&amp;page=&amp;pageSize=</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListProjectsQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _projectService.ListAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ProjectResponseDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ProjectResponseDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.AdminOrManager)]
    public async Task<IActionResult> Create(CreateProjectRequestDto request, CancellationToken cancellationToken)
    {
        var ownerId = CurrentUserIdOrThrow();
        var result = await _projectService.CreateAsync(ownerId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProjectResponseDto>.Ok(result, "Project berhasil dibuat."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOrManager)]
    public async Task<IActionResult> Update(Guid id, UpdateProjectRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _projectService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<ProjectResponseDto>.Ok(result, "Project berhasil diperbarui."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOrManager)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _projectService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok("Project berhasil dihapus."));
    }

    private Guid CurrentUserIdOrThrow() =>
        _currentUserService.UserId ?? throw new UnauthorizedException("Sesi tidak valid.");
}
