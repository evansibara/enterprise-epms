using EPMS.Domain.Common;

namespace EPMS.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    /// <summary>Hash dari refresh token (token mentah tidak pernah disimpan, hanya dikirim ke client).</summary>
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    // Navigation property
    public User? User { get; set; }
}
