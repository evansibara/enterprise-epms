using System.Net;
using EPMS.Application.Common;
using EPMS.Domain.Exceptions;
using FluentValidation;

namespace EPMS.WebApi.Middleware;

/// <summary>
/// Menangkap semua exception yang tidak ditangani controller dan mengubahnya
/// menjadi response JSON konsisten (ApiResponse) sesuai section 5.4 dokumen,
/// supaya axios interceptor di frontend selalu menerima bentuk yang sama.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Validasi gagal.",
                validationEx.Errors.Select(e => e.ErrorMessage).ToList() as IReadOnlyList<string>),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                notFoundEx.Message,
                null),

            DomainValidationException domainEx => (
                HttpStatusCode.BadRequest,
                domainEx.Message,
                null),

            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                unauthorizedEx.Message,
                null),

            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                forbiddenEx.Message,
                null),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                conflictEx.Message,
                null),

            _ => (
                HttpStatusCode.InternalServerError,
                "Terjadi kesalahan internal pada server.",
                null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception terjadi saat memproses request {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogWarning("Request {Path} gagal: {Message}", context.Request.Path, message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.Fail(message, errors);
        await context.Response.WriteAsJsonAsync(response);
    }
}
