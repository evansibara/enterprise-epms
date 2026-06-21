namespace EPMS.Application.DTOs.Users;

public class UserResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

public class CreateUserRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "Employee";
}

public class UpdateUserRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = "Employee";
}
