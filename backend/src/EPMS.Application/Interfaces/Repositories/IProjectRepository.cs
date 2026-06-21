using EPMS.Domain.Entities;
using EPMS.Domain.Enums;

namespace EPMS.Application.Interfaces.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    /// <summary>Ambil project beserta Owner (untuk response detail).</summary>
    Task<Project?> GetByIdWithOwnerAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Project> Items, int TotalCount)> ListAsync(
        string? search,
        ProjectStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
