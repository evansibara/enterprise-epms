using EPMS.Application.DTOs.Comments;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.UseCases.ActivityLogs;
using EPMS.Domain.Entities;
using EPMS.Domain.Exceptions;

namespace EPMS.Application.UseCases.Comments;

public interface ICommentService
{
    Task<CommentResponseDto> CreateAsync(
        Guid taskId, Guid performedByUserId, CreateCommentRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CommentResponseDto>> ListByTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid commentId, Guid performedByUserId, CancellationToken cancellationToken = default);
}

public class CommentService : ICommentService
{
    private const string EntityType = "ProjectTask";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IActivityRecorder _activityRecorder;

    public CommentService(IUnitOfWork unitOfWork, IActivityRecorder activityRecorder)
    {
        _unitOfWork = unitOfWork;
        _activityRecorder = activityRecorder;
    }

    public async Task<CommentResponseDto> CreateAsync(
        Guid taskId, Guid performedByUserId, CreateCommentRequestDto request, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), taskId);

        var comment = new Comment
        {
            TaskId = task.Id,
            Content = request.Content,
            CreatedByUserId = performedByUserId
        };

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
        await _activityRecorder.RecordAsync(EntityType, task.Id, "CommentAdded", performedByUserId, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var author = await _unitOfWork.Users.GetByIdAsync(performedByUserId, cancellationToken);

        return MapToDto(comment, author?.Name);
    }

    public async Task<IReadOnlyList<CommentResponseDto>> ListByTaskAsync(
        Guid taskId, CancellationToken cancellationToken = default)
    {
        var comments = await _unitOfWork.Comments.ListByTaskIdAsync(taskId, cancellationToken);
        return comments.Select(c => MapToDto(c, c.CreatedByUser?.Name)).ToList();
    }

    public async Task DeleteAsync(Guid commentId, Guid performedByUserId, CancellationToken cancellationToken = default)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Comment), commentId);

        // Hanya penulis komentar sendiri yang boleh menghapusnya (cek role Admin
        // dilakukan terpisah di controller lewat policy jika dibutuhkan kebijakan lebih luas).
        if (comment.CreatedByUserId != performedByUserId)
        {
            throw new ForbiddenException("Anda hanya dapat menghapus komentar milik Anda sendiri.");
        }

        _unitOfWork.Comments.SoftDelete(comment);
        await _activityRecorder.RecordAsync(EntityType, comment.TaskId, "CommentRemoved", performedByUserId, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static CommentResponseDto MapToDto(Comment comment, string? authorName) => new()
    {
        Id = comment.Id,
        TaskId = comment.TaskId,
        Content = comment.Content,
        CreatedByUserId = comment.CreatedByUserId,
        CreatedByUserName = authorName,
        CreatedAt = comment.CreatedAt
    };
}
