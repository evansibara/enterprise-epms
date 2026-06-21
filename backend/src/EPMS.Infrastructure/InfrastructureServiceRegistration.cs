using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.Interfaces.Services;
using EPMS.Infrastructure.Auth;
using EPMS.Infrastructure.Caching;
using EPMS.Infrastructure.Files;
using EPMS.Infrastructure.Persistence;
using EPMS.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EPMS.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // --- PostgreSQL / EF Core (section 4.1) ---
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // --- Repository pattern (section 4.3) ---
        // Catatan: semua use case mengakses repository lewat IUnitOfWork
        // (lihat UnitOfWork.cs — lazy-init repository internal), sehingga
        // registrasi IUserRepository/IProjectRepository/dst secara individual
        // tidak diperlukan.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // --- Redis (section 4.4) ---
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IRefreshTokenCacheService, RefreshTokenCacheService>();

        // --- JWT & Password Hashing (section 4.5–4.6) ---
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        // --- Current user accessor ---
        // IHttpContextAccessor diregistrasikan di EPMS.WebApi/Program.cs
        // (builder.Services.AddHttpContextAccessor()), bukan di sini, karena
        // implementasi konkretnya hanya tersedia lewat framework reference
        // ASP.NET Core penuh yang dimiliki project Web, bukan classlib ini.
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // --- File storage (section 4.7) ---
        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}