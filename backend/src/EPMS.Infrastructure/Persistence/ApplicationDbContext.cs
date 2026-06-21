using EPMS.Domain.Common;
using EPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectTask> Tasks => Set<ProjectTask>();

    public DbSet<TaskAttachment> Attachments => Set<TaskAttachment>();

    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter untuk soft delete (section 3.3): baris dengan
        // DeletedAt != null otomatis tersembunyi dari query LINQ biasa.
        // Didaftarkan eksplisit per entity agar mudah dibaca dan type-safe.
        modelBuilder.Entity<User>().HasQueryFilter(e => e.DeletedAt == null);
        modelBuilder.Entity<Project>().HasQueryFilter(e => e.DeletedAt == null);
        modelBuilder.Entity<ProjectTask>().HasQueryFilter(e => e.DeletedAt == null);
        modelBuilder.Entity<TaskAttachment>().HasQueryFilter(e => e.DeletedAt == null);
        modelBuilder.Entity<ActivityLog>().HasQueryFilter(e => e.DeletedAt == null);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(e => e.DeletedAt == null);
    }

    public override int SaveChanges()
    {
        ApplyAuditTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
