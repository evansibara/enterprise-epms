using EPMS.Application.DTOs.Tasks;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.UseCases.ActivityLogs;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Exceptions;
using TaskStatusEnum = EPMS.Domain.Enums.TaskStatus;

namespace EPMS.Application.UseCases.Tasks;

public class TaskService : ITaskService
{
    private const string EntityType = "ProjectTask";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IActivityRecorder _activityRecorder;

    public TaskService(IUnitOfWork unitOfWork, IActivityRecorder activityRecorder)
    {
        _unitOfWork = unitOfWork;
        _activityRecorder = activityRecorder;
    }

    public async Task<TaskResponseDto> CreateAsync(
        Guid projectId, Guid performedByUserId, CreateTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), projectId);

        if (!Enum.TryParse<TaskPriority>(request.Priority, ignoreCase: true, out var priority))
        {
            throw new DomainValidationException($"Priority '{request.Priority}' tidak valid.");
        }

        if (request.AssigneeId.HasValue)
        {
            _ = await _unitOfWork.Users.GetByIdAsync(request.AssigneeId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.AssigneeId.Value);
        }

        var task = new ProjectTask
        {
            ProjectId = project.Id,
            Title = request.Title,
            Description = request.Description,
            AssigneeId = request.AssigneeId,
            Priority = priority,
            Status = TaskStatusEnum.ToDo,
            DueDate = request.DueDate
        };

        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _activityRecorder.RecordAsync(EntityType, task.Id, "Created", performedByUserId, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildDtoAsync(task, cancellationToken);
    }

    public async Task<TaskResponseDto> UpdateAsync(
        Guid taskId, Guid performedByUserId, UpdateTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), taskId);

        if (!Enum.TryParse<TaskPriority>(request.Priority, ignoreCase: true, out var priority))
        {
            throw new DomainValidationException($"Priority '{request.Priority}' tidak valid.");
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = priority;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Tasks.Update(task);
        await _activityRecorder.RecordAsync(EntityType, task.Id, "Updated", performedByUserId, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildDtoAsync(task, cancellationToken);
    }

    public async Task<TaskResponseDto> UpdateStatusAsync(
        Guid taskId, Guid performedByUserId, UpdateTaskStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), taskId);

        if (!Enum.TryParse<TaskStatusEnum>(request.Status, ignoreCase: true, out var newStatus))
        {
            throw new DomainValidationException($"Status '{request.Status}' tidak valid.");
        }

        var previousStatus = task.Status;
        task.Status = newStatus;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Tasks.Update(task);
        await _activityRecorder.RecordAsync(
            EntityType, task.Id, "StatusChanged", performedByUserId,
            metadata: new { from = previousStatus.ToString(), to = newStatus.ToString() },
            cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildDtoAsync(task, cancellationToken);
    }

    public async Task<TaskResponseDto> AssignAsync(
        Guid taskId, Guid performedByUserId, AssignTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), taskId);

        var assignee = await _unitOfWork.Users.GetByIdAsync(request.AssigneeId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.AssigneeId);

        task.AssigneeId = assignee.Id;
        task.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Tasks.Update(task);
        await _activityRecorder.RecordAsync(
            EntityType, task.Id, "Assigned", performedByUserId,
            metadata: new { assigneeId = assignee.Id, assigneeName = assignee.Name },
            cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildDtoAsync(task, cancellationToken);
    }

    public async Task DeleteAsync(Guid taskId, Guid performedByUserId, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), taskId);

        _unitOfWork.Tasks.SoftDelete(task);
        await _activityRecorder.RecordAsync(EntityType, task.Id, "Deleted", performedByUserId, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskResponseDto>> ListByProjectAsync(
        Guid projectId, CancellationToken cancellationToken = default)
    {
        var tasks = await _unitOfWork.Tasks.ListByProjectIdAsync(projectId, cancellationToken);
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<TaskResponseDto> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(taskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), taskId);

        return MapToDto(task);
    }

    private async Task<TaskResponseDto> BuildDtoAsync(ProjectTask task, CancellationToken cancellationToken)
    {
        var detailed = await _unitOfWork.Tasks.GetByIdWithDetailsAsync(task.Id, cancellationToken);
        return MapToDto(detailed ?? task);
    }

    private static TaskResponseDto MapToDto(ProjectTask task) => new()
    {
        Id = task.Id,
        ProjectId = task.ProjectId,
        Title = task.Title,
        Description = task.Description,
        AssigneeId = task.AssigneeId,
        AssigneeName = task.Assignee?.Name,
        Priority = task.Priority.ToString(),
        Status = task.Status.ToString(),
        DueDate = task.DueDate,
        CreatedAt = task.CreatedAt
    };
}
