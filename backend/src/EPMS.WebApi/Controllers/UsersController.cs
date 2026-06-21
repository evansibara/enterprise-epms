using EPMS.Application.Common;
using EPMS.Application.DTOs.Common;
using EPMS.Application.DTOs.Users;
using EPMS.Application.UseCases.Users;
using EPMS.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.WebApi.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>GET /api/v1/users — dipakai juga oleh frontend untuk dropdown Assignee,
    /// jadi sengaja dibuka untuk semua role terautentikasi (bukan AdminOnly).</summary>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.ListAsync(search, page, pageSize, cancellationToken);
        return Ok(ApiResponse<PagedResult<UserResponseDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<UserResponseDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Create(CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<UserResponseDto>.Ok(result, "User berhasil dibuat."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<UserResponseDto>.Ok(result, "User berhasil diperbarui."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse.Ok("User berhasil dihapus."));
    }
}
