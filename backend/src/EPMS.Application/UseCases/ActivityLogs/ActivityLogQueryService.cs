using EPMS.Application.DTOs.ActivityLogs;
using EPMS.Application.Interfaces.Repositories;

namespace EPMS.Application.UseCases.ActivityLogs;

public interface IActivityLogQueryService
{
    Task<IReadOnlyList<ActivityLogResponseDto>> ListByEntityAsync(
        string entityType, Guid entityId, CancellationToken cancellationToken = default);
}

public class ActivityLogQueryService : IActivityLogQueryService
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivityLogQueryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ActivityLogResponseDto>> ListByEntityAsync(
        string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        var logs = await _unitOfWork.ActivityLogs.ListByEntityAsync(entityType, entityId, cancellationToken);

        return logs.Select(l => new ActivityLogResponseDto
        {
            Id = l.Id,
            EntityType = l.EntityType,
            EntityId = l.EntityId,
            Action = l.Action,
            PerformedByUserId = l.PerformedByUserId,
            PerformedByUserName = l.PerformedByUser?.Name,
            Timestamp = l.Timestamp,
            MetadataJson = l.MetadataJson
        }).ToList();
    }
}
