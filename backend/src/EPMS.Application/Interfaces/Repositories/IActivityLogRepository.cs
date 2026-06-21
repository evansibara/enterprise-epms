using EPMS.Domain.Entities;

namespace EPMS.Application.Interfaces.Repositories;

public interface IActivityLogRepository : IRepository<ActivityLog>
{
    Task<IReadOnlyList<ActivityLog>> ListByEntityAsync(
        string entityType, Guid entityId, CancellationToken cancellationToken = default);
}
