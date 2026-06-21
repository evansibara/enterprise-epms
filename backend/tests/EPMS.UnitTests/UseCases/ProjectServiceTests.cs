using EPMS.Application.DTOs.Projects;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.UseCases.ActivityLogs;
using EPMS.Application.UseCases.Projects;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace EPMS.UnitTests.UseCases;

public class ProjectServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IActivityRecorder> _activityRecorderMock = new();

    private readonly ProjectService _sut;

    public ProjectServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);

        _sut = new ProjectService(_unitOfWorkMock.Object, _activityRecorderMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProjectWithPlanningStatus_AndRecordActivity()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var owner = new User { Id = ownerId, Name = "Owner Name" };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(ownerId, It.IsAny<CancellationToken>())).ReturnsAsync(owner);

        var request = new CreateProjectRequestDto { Name = "Project A", Description = "Desc" };

        // Act
        var result = await _sut.CreateAsync(ownerId, request);

        // Assert
        result.Name.Should().Be("Project A");
        result.Status.Should().Be(ProjectStatus.Planning.ToString());
        result.OwnerName.Should().Be("Owner Name");

        _activityRecorderMock.Verify(
            a => a.RecordAsync("Project", It.IsAny<Guid>(), "Created", ownerId, null, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var request = new UpdateProjectRequestDto { Name = "X", Status = "Active" };

        // Act
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowDomainValidation_WhenStatusInvalid()
    {
        // Arrange
        var project = new Project { Id = Guid.NewGuid(), Name = "Existing" };
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var request = new UpdateProjectRequestDto { Name = "Updated", Status = "NotARealStatus" };

        // Act
        var act = async () => await _sut.UpdateAsync(project.Id, request);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDelete_NotHardDelete()
    {
        // Arrange
        var project = new Project { Id = Guid.NewGuid(), OwnerId = Guid.NewGuid() };
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        await _sut.DeleteAsync(project.Id);

        // Assert — verifikasi soft delete dipanggil (bukan hard delete/Remove).
        _projectRepositoryMock.Verify(r => r.SoftDelete(project), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
