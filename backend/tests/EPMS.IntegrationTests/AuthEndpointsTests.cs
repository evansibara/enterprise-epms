using System.Net;
using System.Net.Http.Json;
using EPMS.Application.Common;
using EPMS.Application.DTOs.Auth;
using FluentAssertions;

namespace EPMS.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ThenLogin_ShouldSucceed()
    {
        // Arrange
        var email = $"user-{Guid.NewGuid():N}@example.com";
        var registerRequest = new RegisterRequestDto
        {
            Name = "Integration Test User",
            Email = email,
            Password = "Password123"
        };

        // Act — Register
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Assert — Register
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act — Login
        var loginRequest = new LoginRequestDto { Email = email, Password = "Password123" };
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert — Login
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data!.AccessToken.Should().NotBeNullOrEmpty();
        body.Data.User.Email.Should().Be(email);

        // Refresh token harus dikirim lewat HttpOnly cookie, bukan di body response.
        loginResponse.Headers.Should().Contain(h => h.Key == "Set-Cookie");
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordWrong()
    {
        // Arrange
        var email = $"user-{Guid.NewGuid():N}@example.com";
        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequestDto
        {
            Name = "Another User",
            Email = email,
            Password = "Password123"
        });

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto
        {
            Email = email,
            Password = "WrongPassword999"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Projects_Endpoint_ShouldReturnUnauthorized_WithoutToken()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/projects");

        // Assert — RBAC: endpoint terlindungi [Authorize] harus menolak tanpa token.
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
