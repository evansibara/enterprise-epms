using EPMS.Application.DTOs.Dashboard;
using EPMS.Application.Interfaces.Repositories;
using TaskStatusEnum = EPMS.Domain.Enums.TaskStatus;

namespace EPMS.Application.UseCases.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        // ListAsync dengan pageSize=1 cukup untuk mengambil TotalCount tanpa
        // perlu menarik seluruh baris data — items-nya sendiri tidak dipakai.
        var (_, totalProjects) = await _unitOfWork.Projects.ListAsync(
            search: null, status: null, page: 1, pageSize: 1, cancellationToken);

        var (_, totalUsers) = await _unitOfWork.Users.ListAsync(
            search: null, page: 1, pageSize: 1, cancellationToken);

        // "Aktif" = task yang belum Done (ToDo + InProgress + Review).
        var toDoCount = await _unitOfWork.Tasks.CountByStatusAsync(TaskStatusEnum.ToDo, cancellationToken);
        var inProgressCount = await _unitOfWork.Tasks.CountByStatusAsync(TaskStatusEnum.InProgress, cancellationToken);
        var reviewCount = await _unitOfWork.Tasks.CountByStatusAsync(TaskStatusEnum.Review, cancellationToken);
        var doneCount = await _unitOfWork.Tasks.CountByStatusAsync(TaskStatusEnum.Done, cancellationToken);

        // Ambang batas 3 hari ke depan dianggap "mendekati deadline" untuk widget dashboard.
        var pendingDeadlines = await _unitOfWork.Tasks.CountPendingDeadlinesAsync(3, cancellationToken);

        return new DashboardSummaryDto
        {
            TotalProjects = totalProjects,
            ActiveTasks = toDoCount + inProgressCount + reviewCount,
            CompletedTasks = doneCount,
            TeamMembers = totalUsers,
            PendingDeadlines = pendingDeadlines
        };
    }
}
