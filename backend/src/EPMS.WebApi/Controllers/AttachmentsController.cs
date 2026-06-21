using EPMS.Application.Common;
using EPMS.Application.DTOs.Attachments;
using EPMS.Application.Interfaces.Services;
using EPMS.Application.UseCases.Tasks;
using EPMS.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.WebApi.Controllers;

[ApiController]
[Authorize]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly ICurrentUserService _currentUserService;

    public AttachmentsController(IAttachmentService attachmentService, ICurrentUserService currentUserService)
    {
        _attachmentService = attachmentService;
        _currentUserService = currentUserService;
    }

    /// <summary>GET /api/v1/tasks/{taskId}/attachments</summary>
    [HttpGet("api/v1/tasks/{taskId:guid}/attachments")]
    public async Task<IActionResult> ListByTask(Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _attachmentService.ListByTaskAsync(taskId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AttachmentResponseDto>>.Ok(result));
    }

    /// <summary>POST /api/v1/tasks/{taskId}/attachments (multipart/form-data, field "file")</summary>
    [HttpPost("api/v1/tasks/{taskId:guid}/attachments")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB, samakan dengan FileStorageSettings.MaxFileSizeBytes
    public async Task<IActionResult> Upload(Guid taskId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse.Fail("File wajib diupload."));
        }

        var userId = CurrentUserIdOrThrow();

        await using var stream = file.OpenReadStream();
        var result = await _attachmentService.UploadAsync(
            taskId, userId, stream, file.FileName, file.ContentType, file.Length, cancellationToken);

        return Ok(ApiResponse<AttachmentResponseDto>.Ok(result, "File berhasil diupload."));
    }

    /// <summary>GET /api/v1/attachments/{id}/download</summary>
    [HttpGet("api/v1/attachments/{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await _attachmentService.DownloadAsync(id, cancellationToken);
        return File(result.Content, result.MimeType, result.FileName);
    }

    [HttpDelete("api/v1/attachments/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = CurrentUserIdOrThrow();
        await _attachmentService.DeleteAsync(id, userId, cancellationToken);
        return Ok(ApiResponse.Ok("Attachment berhasil dihapus."));
    }

    private Guid CurrentUserIdOrThrow() =>
        _currentUserService.UserId ?? throw new UnauthorizedException("Sesi tidak valid.");
}
