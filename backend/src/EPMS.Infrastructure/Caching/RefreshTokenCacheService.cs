using EPMS.Application.Interfaces.Services;

namespace EPMS.Infrastructure.Caching;

/// <summary>
/// Lapisan tambahan di atas ICacheService khusus untuk refresh token, sesuai
/// section 4.4: "Simpan refresh token (untuk revocation cepat)... di Redis".
/// Sumber kebenaran (source of truth) tetap tabel RefreshTokens di PostgreSQL;
/// Redis di sini hanya mempercepat pengecekan "apakah token ini revoked?"
/// tanpa perlu round-trip ke database pada setiap request berautentikasi.
/// </summary>
public interface IRefreshTokenCacheService
{
    Task MarkActiveAsync(string tokenHash, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task<bool> IsActiveAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task RevokeAsync(string tokenHash, CancellationToken cancellationToken = default);
}

public class RefreshTokenCacheService : IRefreshTokenCacheService
{
    private const string KeyPrefix = "refresh-token:active:";
    private readonly ICacheService _cacheService;

    public RefreshTokenCacheService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task MarkActiveAsync(string tokenHash, TimeSpan ttl, CancellationToken cancellationToken = default) =>
        _cacheService.SetStringAsync(BuildKey(tokenHash), "1", ttl, cancellationToken);

    public Task<bool> IsActiveAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _cacheService.ExistsAsync(BuildKey(tokenHash), cancellationToken);

    public Task RevokeAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _cacheService.RemoveAsync(BuildKey(tokenHash), cancellationToken);

    private static string BuildKey(string tokenHash) => $"{KeyPrefix}{tokenHash}";
}
