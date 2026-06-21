using EPMS.Application.Interfaces.Repositories;
using EPMS.Domain.Entities;
using EPMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Persistence.Repositories;

public class ActivityLogRepository : RepositoryBase<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ActivityLog>> ListByEntityAsync(
        string entityType, Guid entityId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(a => a.PerformedByUser)
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
}
