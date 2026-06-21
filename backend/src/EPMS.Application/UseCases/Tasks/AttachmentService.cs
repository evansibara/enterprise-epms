using EPMS.Application.DTOs.Attachments;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.Interfaces.Services;
using EPMS.Application.UseCases.ActivityLogs;
using EPMS.Domain.Entities;
using EPMS.Domain.Exceptions;

namespace EPMS.Application.UseCases.Tasks;

public record DownloadFileResult(Stream Content, string FileName, string MimeType);

public interface IAttachmentService
{
    Task<AttachmentResponseDto> UploadAsync(
        Guid taskId, Guid performedByUserId, Stream fileStream, string fileName, string mimeType, long sizeBytes,
        CancellationToken cancellationToken = default);

    Task<DownloadFileResult> DownloadAsync(Guid attachmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AttachmentResponseDto>> ListByTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid attachmentId, Guid performedByUserId, CancellationToken cancellationToken = default);
}

public class AttachmentService : IAttachmentService
{
    private const string EntityType = "ProjectTask";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IActivityRecorder _activityRecorder;

    public AttachmentService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IActivityRecorder activityRecorder)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _activityRecorder = activityRecorder;
    }

    public async Task<AttachmentResponseDto> UploadAsync(
        Guid taskId, Guid performedByUserId, Stream fileStream, string fileName, string mimeType, long sizeBytes,
        CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), taskId);

        // Validasi MIME type & ukuran dilakukan di dalam IFileStorageService (section 4.7).
        var stored = await _fileStorageService.SaveAsync(fileStream, fileName, mimeType, sizeBytes, cancellationToken);

        var attachment = new TaskAttachment
        {
            TaskId = task.Id,
            FileName = stored.FileName,
            FilePath = stored.FilePath,
            MimeType = stored.MimeType,
            SizeBytes = stored.SizeBytes
        };

        await _unitOfWork.Attachments.AddAsync(attachment, cancellationToken);
        await _activityRecorder.RecordAsync(
            EntityType, task.Id, "AttachmentAdded", performedByUserId,
            metadata: new { fileName = attachment.FileName },
            cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(attachment);
    }

    public async Task<DownloadFileResult> DownloadAsync(Guid attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _unitOfWork.Attachments.GetByIdAsync(attachmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskAttachment), attachmentId);

        var stream = await _fileStorageService.GetAsync(attachment.FilePath, cancellationToken);
        return new DownloadFileResult(stream, attachment.FileName, attachment.MimeType);
    }

    public async Task<IReadOnlyList<AttachmentResponseDto>> ListByTaskAsync(
        Guid taskId, CancellationToken cancellationToken = default)
    {
        var attachments = await _unitOfWork.Attachments.ListByTaskIdAsync(taskId, cancellationToken);
        return attachments.Select(MapToDto).ToList();
    }

    public async Task DeleteAsync(Guid attachmentId, Guid performedByUserId, CancellationToken cancellationToken = default)
    {
        var attachment = await _unitOfWork.Attachments.GetByIdAsync(attachmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(TaskAttachment), attachmentId);

        await _fileStorageService.DeleteAsync(attachment.FilePath, cancellationToken);
        _unitOfWork.Attachments.SoftDelete(attachment);

        await _activityRecorder.RecordAsync(
            EntityType, attachment.TaskId, "AttachmentRemoved", performedByUserId,
            metadata: new { fileName = attachment.FileName },
            cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static AttachmentResponseDto MapToDto(TaskAttachment attachment) => new()
    {
        Id = attachment.Id,
        TaskId = attachment.TaskId,
        FileName = attachment.FileName,
        MimeType = attachment.MimeType,
        SizeBytes = attachment.SizeBytes,
        CreatedAt = attachment.CreatedAt
    };
}
