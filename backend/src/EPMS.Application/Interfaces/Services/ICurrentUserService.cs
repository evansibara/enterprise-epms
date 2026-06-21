using EPMS.Domain.Enums;

namespace EPMS.Application.Interfaces.Services;

/// <summary>
/// Abstraksi untuk mengambil identitas user yang sedang login (dari JWT claims)
/// tanpa Application layer perlu tahu tentang HttpContext/ASP.NET Core.
/// Diimplementasikan di Infrastructure/WebApi layer.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }

    UserRole? Role { get; }

    bool IsAuthenticated { get; }
}
