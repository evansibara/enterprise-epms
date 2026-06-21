using EPMS.Domain.Common;
using EPMS.Domain.Enums;

namespace EPMS.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Employee;

    // Navigation properties
    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();

    public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
