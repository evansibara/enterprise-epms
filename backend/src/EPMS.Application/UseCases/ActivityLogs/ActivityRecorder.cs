using System.Text.Json;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Domain.Entities;

namespace EPMS.Application.UseCases.ActivityLogs;

public class ActivityRecorder : IActivityRecorder
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivityRecorder(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task RecordAsync(
        string entityType,
        Guid entityId,
        string action,
        Guid performedByUserId,
        object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var log = new ActivityLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            PerformedByUserId = performedByUserId,
            Timestamp = DateTime.UtcNow,
            MetadataJson = metadata is null ? null : JsonSerializer.Serialize(metadata)
        };

        // Sengaja TIDAK memanggil SaveChangesAsync di sini — caller (use case
        // Project/Task) yang memanggil SaveChangesAsync sekali di akhir, supaya
        // perubahan entity + activity log tercatat dalam satu transaksi atomik.
        await _unitOfWork.ActivityLogs.AddAsync(log, cancellationToken);
    }
}
