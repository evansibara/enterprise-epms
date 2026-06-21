using EPMS.Application.Interfaces.Repositories;
using EPMS.Infrastructure.Persistence;

namespace EPMS.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IUserRepository? _users;
    private IProjectRepository? _projects;
    private ITaskRepository? _tasks;
    private ITaskAttachmentRepository? _attachments;
    private IActivityLogRepository? _activityLogs;
    private IRefreshTokenRepository? _refreshTokens;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);

    public IProjectRepository Projects => _projects ??= new ProjectRepository(_context);

    public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);

    public ITaskAttachmentRepository Attachments => _attachments ??= new TaskAttachmentRepository(_context);

    public IActivityLogRepository ActivityLogs => _activityLogs ??= new ActivityLogRepository(_context);

    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
