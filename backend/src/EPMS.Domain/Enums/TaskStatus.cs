namespace EPMS.Domain.Enums;

/// <summary>
/// Status task, dipakai untuk kolom Kanban di frontend (To Do / In Progress / Review / Done).
/// </summary>
public enum TaskStatus
{
    ToDo = 0,
    InProgress = 1,
    Review = 2,
    Done = 3
}
