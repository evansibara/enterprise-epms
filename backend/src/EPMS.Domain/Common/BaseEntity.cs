namespace EPMS.Domain.Common;

/// <summary>
/// Base class untuk semua entity. Menyediakan Id, CreatedAt, dan DeletedAt
/// (soft delete) sesuai section 3.3 spesifikasi.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Jika tidak null, entity dianggap sudah dihapus (soft delete).
    /// Query filter global akan otomatis menyembunyikan baris ini.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    public bool IsDeleted => DeletedAt != null;
}
