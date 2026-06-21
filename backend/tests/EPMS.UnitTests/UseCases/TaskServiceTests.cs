using EPMS.Application.DTOs.Tasks;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.UseCases.ActivityLogs;
using EPMS.Application.UseCases.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Exceptions;
using FluentAssertions;
using Moq;
using TaskStatusEnum = EPMS.Domain.Enums.TaskStatus;

namespace EPMS.UnitTests.UseCases;

public class TaskServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITaskRepository> _taskRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IActivityRecorder> _activityRecorderMock = new();

    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Tasks).Returns(_taskRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);

        _sut = new TaskService(_unitOfWorkMock.Object, _activityRecorderMock.Object);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus_AndRecordActivityWithFromTo()
    {
        // Arrange
        var task = new ProjectTask { Id = Guid.NewGuid(), Status = TaskStatusEnum.ToDo };
        var performedBy = Guid.NewGuid();

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _taskRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var request = new UpdateTaskStatusRequestDto { Status = "InProgress" };

        // Act
        var result = await _sut.UpdateStatusAsync(task.Id, performedBy, request);

        // Assert
        task.Status.Should().Be(TaskStatusEnum.InProgress);
        result.Status.Should().Be(TaskStatusEnum.InProgress.ToString());

        _activityRecorderMock.Verify(a => a.RecordAsync(
            "ProjectTask", task.Id, "StatusChanged", performedBy,
            It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldThrowDomainValidation_WhenStatusInvalid()
    {
        // Arrange
        var task = new ProjectTask { Id = Guid.NewGuid() };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var request = new UpdateTaskStatusRequestDto { Status = "NotAStatus" };

        // Act
        var act = async () => await _sut.UpdateStatusAsync(task.Id, Guid.NewGuid(), request);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>();
    }

    [Fact]
    public async Task AssignAsync_ShouldThrowNotFound_WhenAssigneeDoesNotExist()
    {
        // Arrange
        var task = new ProjectTask { Id = Guid.NewGuid() };
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var request = new AssignTaskRequestDto { AssigneeId = Guid.NewGuid() };

        // Act
        var act = async () => await _sut.AssignAsync(task.Id, Guid.NewGuid(), request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
