using EPMS.Application.Common;
using EPMS.Application.DTOs.Auth;
using EPMS.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EPMS.WebApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "epms_refresh_token";

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        SetRefreshTokenCookie(result.PlainRefreshToken, result.RefreshTokenExpiresAt);

        return Ok(ApiResponse<LoginResponseDto>.Ok(result.LoginResponse, "Login berhasil."));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(ApiResponse<RegisterResponseDto>.Ok(result, "Registrasi berhasil."));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var plainRefreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrEmpty(plainRefreshToken))
        {
            return Unauthorized(ApiResponse.Fail("Refresh token tidak ditemukan."));
        }

        var result = await _authService.RefreshTokenAsync(plainRefreshToken, cancellationToken);

        SetRefreshTokenCookie(result.NewPlainRefreshToken, result.NewRefreshTokenExpiresAt);

        return Ok(ApiResponse<RefreshTokenResponseDto>.Ok(result.Response, "Token berhasil di-refresh."));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var plainRefreshToken = Request.Cookies[RefreshTokenCookieName];

        if (!string.IsNullOrEmpty(plainRefreshToken))
        {
            await _authService.LogoutAsync(plainRefreshToken, cancellationToken);
        }

        Response.Cookies.Delete(RefreshTokenCookieName);

        return Ok(ApiResponse.Ok("Logout berhasil."));
    }

    private void SetRefreshTokenCookie(string plainToken, DateTime expiresAt)
    {
        Response.Cookies.Append(RefreshTokenCookieName, plainToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt,
            Path = "/api/v1/auth"
        });
    }
}
