using EPMS.Domain.Entities;

namespace EPMS.Application.Interfaces.Repositories;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IReadOnlyList<Comment>> ListByTaskIdAsync(
        Guid taskId, CancellationToken cancellationToken = default);
}
