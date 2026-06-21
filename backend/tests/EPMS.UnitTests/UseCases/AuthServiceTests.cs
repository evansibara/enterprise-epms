using EPMS.Application.DTOs.Auth;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.Interfaces.Services;
using EPMS.Application.UseCases.Auth;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace EPMS.UnitTests.UseCases;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();

    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.RefreshTokens).Returns(_refreshTokenRepositoryMock.Object);

        _sut = new AuthService(_unitOfWorkMock.Object, _passwordHasherMock.Object, _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsValid()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            Name = "Test User",
            PasswordHash = "hashed-password",
            Role = UserRole.Employee
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify("correct-password", user.PasswordHash)).Returns(true);
        _jwtTokenServiceMock.Setup(j => j.GenerateAccessToken(user))
            .Returns(new AccessTokenResult("access-token", DateTime.UtcNow.AddMinutes(15)));
        _jwtTokenServiceMock.Setup(j => j.GenerateRefreshToken())
            .Returns(new RefreshTokenResult("plain-refresh-token", DateTime.UtcNow.AddDays(7)));
        _jwtTokenServiceMock.Setup(j => j.HashToken("plain-refresh-token")).Returns("hashed-refresh-token");

        var request = new LoginRequestDto { Email = user.Email, Password = "correct-password" };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.LoginResponse.AccessToken.Should().Be("access-token");
        result.LoginResponse.User.Email.Should().Be(user.Email);
        result.PlainRefreshToken.Should().Be("plain-refresh-token");

        _refreshTokenRepositoryMock.Verify(
            r => r.AddAsync(It.Is<RefreshToken>(t => t.UserId == user.Id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenPasswordIncorrect()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "user@example.com", PasswordHash = "hashed" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify("wrong-password", user.PasswordHash)).Returns(false);

        var request = new LoginRequestDto { Email = user.Email, Password = "wrong-password" };

        // Act
        var act = async () => await _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var request = new LoginRequestDto { Email = "ghost@example.com", Password = "whatever" };

        // Act
        var act = async () => await _sut.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.EmailExistsAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new RegisterRequestDto
        {
            Name = "Someone",
            Email = "existing@example.com",
            Password = "Password123"
        };

        // Act
        var act = async () => await _sut.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task RegisterAsync_ShouldAlwaysAssignEmployeeRole_RegardlessOfRequestedRole()
    {
        // Arrange — Catatan keamanan: pastikan tidak ada jalur self-elevation ke Admin.
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasherMock.Setup(p => p.Hash(It.IsAny<string>())).Returns("hashed-password");

        User? capturedUser = null;
        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => capturedUser = u)
            .Returns(Task.CompletedTask);

        var request = new RegisterRequestDto
        {
            Name = "Hacker",
            Email = "hacker@example.com",
            Password = "Password123",
            Role = "Admin" // mencoba self-elevate
        };

        // Act
        await _sut.RegisterAsync(request);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser!.Role.Should().Be(UserRole.Employee);
    }
}
