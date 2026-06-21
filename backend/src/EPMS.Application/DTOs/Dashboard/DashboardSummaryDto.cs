namespace EPMS.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalProjects { get; set; }

    public int ActiveTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int TeamMembers { get; set; }
}
