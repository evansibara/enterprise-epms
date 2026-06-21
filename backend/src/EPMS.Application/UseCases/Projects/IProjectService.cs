using EPMS.Application.DTOs.Common;
using EPMS.Application.DTOs.Projects;

namespace EPMS.Application.UseCases.Projects;

public interface IProjectService
{
    Task<ProjectResponseDto> CreateAsync(
        Guid ownerId, CreateProjectRequestDto request, CancellationToken cancellationToken = default);

    Task<ProjectResponseDto> UpdateAsync(
        Guid projectId, UpdateProjectRequestDto request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<ProjectResponseDto> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<PagedResult<ProjectResponseDto>> ListAsync(
        ListProjectsQueryDto query, CancellationToken cancellationToken = default);
}
