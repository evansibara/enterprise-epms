namespace EPMS.Application.Common;

/// <summary>
/// Format response standar agar mudah dikonsumsi frontend (axios interceptor
/// di shared/api/axios-instance.ts, lihat section 5.4 dokumen).
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }

    public T? Data { get; init; }

    public string? Message { get; init; }

    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}

/// <summary>Variant tanpa payload data, untuk aksi seperti delete/logout.</summary>
public class ApiResponse
{
    public bool Success { get; init; }

    public string? Message { get; init; }

    public IReadOnlyList<string>? Errors { get; init; }

    public static ApiResponse Ok(string? message = null) =>
        new() { Success = true, Message = message };

    public static ApiResponse Fail(string message, IReadOnlyList<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}
