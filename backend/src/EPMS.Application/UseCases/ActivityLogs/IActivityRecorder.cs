namespace EPMS.Application.UseCases.ActivityLogs;

/// <summary>
/// Helper yang dipanggil otomatis dari use case Project/Task lain setiap ada
/// perubahan (create/update/delete/status change), sesuai section 3 dokumen:
/// "RecordActivity (dipanggil otomatis tiap ada perubahan Task/Project)".
/// </summary>
public interface IActivityRecorder
{
    Task RecordAsync(
        string entityType,
        Guid entityId,
        string action,
        Guid performedByUserId,
        object? metadata = null,
        CancellationToken cancellationToken = default);
}
