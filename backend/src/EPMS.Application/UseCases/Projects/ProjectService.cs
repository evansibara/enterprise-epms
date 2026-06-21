using EPMS.Application.DTOs.Common;
using EPMS.Application.DTOs.Projects;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.UseCases.ActivityLogs;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Exceptions;

namespace EPMS.Application.UseCases.Projects;

public class ProjectService : IProjectService
{
    private const string EntityType = "Project";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IActivityRecorder _activityRecorder;

    public ProjectService(IUnitOfWork unitOfWork, IActivityRecorder activityRecorder)
    {
        _unitOfWork = unitOfWork;
        _activityRecorder = activityRecorder;
    }

    public async Task<ProjectResponseDto> CreateAsync(
        Guid ownerId, CreateProjectRequestDto request, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Deadline = request.Deadline,
            Status = ProjectStatus.Planning,
            OwnerId = ownerId
        };

        await _unitOfWork.Projects.AddAsync(project, cancellationToken);
        await _activityRecorder.RecordAsync(EntityType, project.Id, "Created", ownerId, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var owner = await _unitOfWork.Users.GetByIdAsync(ownerId, cancellationToken);
        return MapToDto(project, owner?.Name);
    }

    public async Task<ProjectResponseDto> UpdateAsync(
        Guid projectId, UpdateProjectRequestDto request, CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), projectId);

        if (!Enum.TryParse<ProjectStatus>(request.Status, ignoreCase: true, out var newStatus))
        {
            throw new DomainValidationException($"Status '{request.Status}' tidak valid.");
        }

        var previousStatus = project.Status;

        project.Name = request.Name;
        project.Description = request.Description;
        project.Deadline = request.Deadline;
        project.Status = newStatus;
        project.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Projects.Update(project);

        await _activityRecorder.RecordAsync(
            EntityType, project.Id, "Updated", project.OwnerId,
            metadata: new { previousStatus = previousStatus.ToString(), newStatus = newStatus.ToString() },
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var owner = await _unitOfWork.Users.GetByIdAsync(project.OwnerId, cancellationToken);
        return MapToDto(project, owner?.Name);
    }

    public async Task DeleteAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), projectId);

        _unitOfWork.Projects.SoftDelete(project);

        await _activityRecorder.RecordAsync(EntityType, project.Id, "Deleted", project.OwnerId, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProjectResponseDto> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects.GetByIdWithOwnerAsync(projectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), projectId);

        return MapToDto(project, project.Owner?.Name);
    }

    public async Task<PagedResult<ProjectResponseDto>> ListAsync(
        ListProjectsQueryDto query, CancellationToken cancellationToken = default)
    {
        ProjectStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (!Enum.TryParse<ProjectStatus>(query.Status, ignoreCase: true, out var parsed))
            {
                throw new DomainValidationException($"Status '{query.Status}' tidak valid.");
            }
            statusFilter = parsed;
        }

        var (items, totalCount) = await _unitOfWork.Projects.ListAsync(
            query.Search, statusFilter, query.Page, query.PageSize, cancellationToken);

        var dtos = items.Select(p => MapToDto(p, p.Owner?.Name)).ToList();

        return PagedResult<ProjectResponseDto>.Create(dtos, totalCount, query.Page, query.PageSize);
    }

    private static ProjectResponseDto MapToDto(Project project, string? ownerName) => new()
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        Deadline = project.Deadline,
        Status = project.Status.ToString(),
        OwnerId = project.OwnerId,
        OwnerName = ownerName,
        CreatedAt = project.CreatedAt
    };
}
