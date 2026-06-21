using EPMS.Application.Interfaces.Repositories;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Persistence.Repositories;

public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Project?> GetByIdWithOwnerAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Project> Items, int TotalCount)> ListAsync(
        string? search,
        ProjectStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(p => p.Owner).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term)
                || (p.Description != null && p.Description.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
