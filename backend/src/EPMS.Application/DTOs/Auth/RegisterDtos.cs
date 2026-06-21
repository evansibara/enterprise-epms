namespace EPMS.Application.DTOs.Auth;

public class RegisterRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Role opsional saat register. Default Employee jika tidak diisi.
    /// Hanya Admin yang boleh membuat user dengan role lain lewat endpoint
    /// terpisah (UsersController) — endpoint register publik selalu Employee.
    /// </summary>
    public string? Role { get; set; }
}

public class RegisterResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
