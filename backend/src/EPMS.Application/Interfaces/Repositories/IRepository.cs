using EPMS.Domain.Common;

namespace EPMS.Application.Interfaces.Repositories;

/// <summary>Operasi CRUD generic yang dipakai semua repository spesifik.</summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    /// <summary>Soft delete: mengisi DeletedAt, tidak menghapus baris fisik.</summary>
    void SoftDelete(T entity);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
