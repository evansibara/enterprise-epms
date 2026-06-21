using EPMS.Application.DTOs.Auth;

namespace EPMS.Application.UseCases.Auth;

public record AuthTokensResult(LoginResponseDto LoginResponse, string PlainRefreshToken, DateTime RefreshTokenExpiresAt);

public record RefreshResult(RefreshTokenResponseDto Response, string NewPlainRefreshToken, DateTime NewRefreshTokenExpiresAt);

/// <summary>
/// Kontrak use case Auth: Login, Register, RefreshToken, Logout (section 3).
/// Refresh token mentah dikembalikan ke caller (Controller) supaya Controller
/// yang bertanggung jawab menulisnya ke HttpOnly Cookie — Application layer
/// tidak boleh tahu tentang HttpContext.
/// </summary>
public interface IAuthService
{
    Task<AuthTokensResult> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<RefreshResult> RefreshTokenAsync(string plainRefreshToken, CancellationToken cancellationToken = default);

    Task LogoutAsync(string plainRefreshToken, CancellationToken cancellationToken = default);
}
