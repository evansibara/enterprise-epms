namespace EPMS.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalProjects { get; set; }

    public int ActiveTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int TeamMembers { get; set; }

    /// <summary>Task yang belum Done dan tenggat waktunya dalam 3 hari ke depan (termasuk overdue).</summary>
    public int PendingDeadlines { get; set; }
}
