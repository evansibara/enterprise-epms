using EPMS.Domain.Entities;

namespace EPMS.Application.Interfaces.Services;

public record AccessTokenResult(string Token, DateTime ExpiresAt);

public record RefreshTokenResult(string PlainToken, DateTime ExpiresAt);

public interface IJwtTokenService
{
    /// <summary>Buat access token JWT (expiry pendek, section 3.4: 15 menit).</summary>
    AccessTokenResult GenerateAccessToken(User user);

    /// <summary>Buat refresh token mentah (random string) + tanggal expired.
    /// Token mentah ini yang dikirim ke client lewat HttpOnly cookie;
    /// hash-nya yang disimpan di database/Redis.</summary>
    RefreshTokenResult GenerateRefreshToken();

    string HashToken(string plainToken);
}
