using EPMS.Application.Interfaces.Repositories;
using EPMS.Domain.Entities;
using EPMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Persistence.Repositories;

public class TaskAttachmentRepository : RepositoryBase<TaskAttachment>, ITaskAttachmentRepository
{
    public TaskAttachmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<TaskAttachment>> ListByTaskIdAsync(
        Guid taskId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(a => a.TaskId == taskId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
}
