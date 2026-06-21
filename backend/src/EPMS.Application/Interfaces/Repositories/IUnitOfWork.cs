namespace EPMS.Application.Interfaces.Repositories;

/// <summary>
/// Mengoordinasikan transaksi antar beberapa repository dalam satu use case
/// (misal: update Task + tulis ActivityLog harus commit bersamaan).
/// </summary>
public interface IUnitOfWork
{
    IUserRepository Users { get; }

    IProjectRepository Projects { get; }

    ITaskRepository Tasks { get; }

    ITaskAttachmentRepository Attachments { get; }

    IActivityLogRepository ActivityLogs { get; }

    IRefreshTokenRepository RefreshTokens { get; }

    ICommentRepository Comments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
