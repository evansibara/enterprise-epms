using EPMS.Domain.Entities;

namespace EPMS.Application.Interfaces.Repositories;

public interface ITaskAttachmentRepository : IRepository<TaskAttachment>
{
    Task<IReadOnlyList<TaskAttachment>> ListByTaskIdAsync(
        Guid taskId, CancellationToken cancellationToken = default);
}
