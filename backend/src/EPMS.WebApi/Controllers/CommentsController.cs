using EPMS.Application.Common;
using EPMS.Application.DTOs.Comments;
using EPMS.Application.Interfaces.Services;
using EPMS.Application.UseCases.Comments;
using EPMS.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.WebApi.Controllers;

[ApiController]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ICurrentUserService _currentUserService;

    public CommentsController(ICommentService commentService, ICurrentUserService currentUserService)
    {
        _commentService = commentService;
        _currentUserService = currentUserService;
    }

    /// <summary>GET /api/v1/tasks/{taskId}/comments</summary>
    [HttpGet("api/v1/tasks/{taskId:guid}/comments")]
    public async Task<IActionResult> ListByTask(Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _commentService.ListByTaskAsync(taskId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CommentResponseDto>>.Ok(result));
    }

    /// <summary>POST /api/v1/tasks/{taskId}/comments</summary>
    [HttpPost("api/v1/tasks/{taskId:guid}/comments")]
    public async Task<IActionResult> Create(Guid taskId, CreateCommentRequestDto request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserIdOrThrow();
        var result = await _commentService.CreateAsync(taskId, userId, request, cancellationToken);
        return Ok(ApiResponse<CommentResponseDto>.Ok(result, "Komentar berhasil ditambahkan."));
    }

    /// <summary>DELETE /api/v1/comments/{id}</summary>
    [HttpDelete("api/v1/comments/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = CurrentUserIdOrThrow();
        await _commentService.DeleteAsync(id, userId, cancellationToken);
        return Ok(ApiResponse.Ok("Komentar berhasil dihapus."));
    }

    private Guid CurrentUserIdOrThrow() =>
        _currentUserService.UserId ?? throw new UnauthorizedException("Sesi tidak valid.");
}
