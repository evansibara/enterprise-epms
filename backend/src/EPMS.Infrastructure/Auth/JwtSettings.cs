namespace EPMS.Infrastructure.Auth;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>Expiry access token dalam menit. Default 15 sesuai section 3.4.</summary>
    public int AccessTokenExpiryMinutes { get; set; } = 15;

    /// <summary>Expiry refresh token dalam hari.</summary>
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
