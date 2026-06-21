using System.Security.Claims;
using EPMS.Application.Interfaces.Services;
using EPMS.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace EPMS.Infrastructure.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var value = GetClaimValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => GetClaimValue(ClaimTypes.Email);

    public UserRole? Role
    {
        get
        {
            var value = GetClaimValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(value, ignoreCase: true, out var role) ? role : null;
        }
    }

    // Diimplementasikan manual dengan LINQ ke Claims (System.Security.Claims,
    // bawaan BCL) — bukan lewat extension method FindFirstValue dari
    // Microsoft.AspNetCore.Authentication, karena EPMS.Infrastructure adalah
    // classlib biasa yang tidak punya framework reference ASP.NET Core penuh
    // seperti EPMS.WebApi.
    private string? GetClaimValue(string claimType) =>
        Principal?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
}