using EPMS.Domain.Entities;
using TaskStatusEnum = EPMS.Domain.Enums.TaskStatus;

namespace EPMS.Application.Interfaces.Repositories;

public interface ITaskRepository : IRepository<ProjectTask>
{
    Task<ProjectTask?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectTask>> ListByProjectIdAsync(
        Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>Hitung jumlah task lintas seluruh project berdasarkan status, untuk dashboard summary.</summary>
    Task<int> CountByStatusAsync(TaskStatusEnum status, CancellationToken cancellationToken = default);
}
