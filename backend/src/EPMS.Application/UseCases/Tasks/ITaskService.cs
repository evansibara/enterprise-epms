using EPMS.Application.DTOs.Tasks;

namespace EPMS.Application.UseCases.Tasks;

public interface ITaskService
{
    Task<TaskResponseDto> CreateAsync(
        Guid projectId, Guid performedByUserId, CreateTaskRequestDto request, CancellationToken cancellationToken = default);

    Task<TaskResponseDto> UpdateAsync(
        Guid taskId, Guid performedByUserId, UpdateTaskRequestDto request, CancellationToken cancellationToken = default);

    Task<TaskResponseDto> UpdateStatusAsync(
        Guid taskId, Guid performedByUserId, UpdateTaskStatusRequestDto request, CancellationToken cancellationToken = default);

    Task<TaskResponseDto> AssignAsync(
        Guid taskId, Guid performedByUserId, AssignTaskRequestDto request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid taskId, Guid performedByUserId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskResponseDto>> ListByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<TaskResponseDto> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
}
