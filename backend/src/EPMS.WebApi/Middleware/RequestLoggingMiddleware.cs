using System.Diagnostics;

namespace EPMS.WebApi.Middleware;

/// <summary>
/// Mencatat setiap request masuk (method, path, status code, durasi, dan
/// user yang sedang login jika ada) sebagai audit trail dasar (section 5.2).
/// Untuk audit detail per-entity, lihat ActivityLog (section 3 dan
/// ActivityLogsController).
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

        _logger.LogInformation(
            "{Method} {Path} -> {StatusCode} ({ElapsedMs}ms) by user {UserId}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            userId);
    }
}
