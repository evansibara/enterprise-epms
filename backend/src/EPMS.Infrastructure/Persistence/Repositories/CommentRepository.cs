using EPMS.Application.Interfaces.Repositories;
using EPMS.Domain.Entities;
using EPMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Persistence.Repositories;

public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Comment>> ListByTaskIdAsync(
        Guid taskId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(c => c.CreatedByUser)
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
}
