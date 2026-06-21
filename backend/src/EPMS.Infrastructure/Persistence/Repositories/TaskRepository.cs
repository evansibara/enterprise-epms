using EPMS.Application.Interfaces.Repositories;
using EPMS.Domain.Entities;
using EPMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TaskStatusEnum = EPMS.Domain.Enums.TaskStatus;

namespace EPMS.Infrastructure.Persistence.Repositories;

public class TaskRepository : RepositoryBase<ProjectTask>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProjectTask?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(t => t.Assignee)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProjectTask>> ListByProjectIdAsync(
        Guid projectId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(t => t.Assignee)
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<int> CountByStatusAsync(
        TaskStatusEnum status, CancellationToken cancellationToken = default) =>
        await DbSet.CountAsync(t => t.Status == status, cancellationToken);
}
