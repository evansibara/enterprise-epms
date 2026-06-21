using EPMS.Application.UseCases.ActivityLogs;
using EPMS.Application.UseCases.Auth;
using EPMS.Application.UseCases.Dashboard;
using EPMS.Application.UseCases.Projects;
using EPMS.Application.UseCases.Tasks;
using EPMS.Application.UseCases.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EPMS.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IActivityRecorder, ActivityRecorder>();
        services.AddScoped<IActivityLogQueryService, ActivityLogQueryService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}

/// <summary>Marker class kosong, dipakai sebagai anchor assembly untuk AddValidatorsFromAssemblyContaining.</summary>
public class ApplicationAssemblyMarker
{
}
