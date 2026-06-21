using EPMS.Application.DTOs.Auth;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.Interfaces.Services;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Exceptions;

namespace EPMS.Application.UseCases.Auth;

public class AuthService : IAuthService
{
    // Sesuai section 3.4: access token 15 menit, refresh token 7 hari.
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthTokensResult> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedException();

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException();
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = await IssueRefreshTokenAsync(user.Id, cancellationToken);

        var response = new LoginResponseDto
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            User = new UserSummaryDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            }
        };

        return new AuthTokensResult(response, refreshToken.PlainToken, refreshToken.ExpiresAt);
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("Email sudah terdaftar.");
        }

        // Catatan keamanan: endpoint register publik SELALU membuat user
        // dengan role Employee, terlepas dari apa pun yang dikirim client,
        // agar tidak ada jalur self-elevation ke Admin/Manager.
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Employee
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<RefreshResult> RefreshTokenAsync(string plainRefreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = _jwtTokenService.HashToken(plainRefreshToken);
        var existingToken = await _unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (existingToken is null || !existingToken.IsActive)
        {
            throw new UnauthorizedException("Refresh token tidak valid atau sudah expired.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(existingToken.UserId, cancellationToken)
            ?? throw new UnauthorizedException();

        // Rotation: revoke token lama, buat token baru (section 4.5).
        existingToken.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(existingToken);

        var newRefreshToken = await IssueRefreshTokenAsync(user.Id, cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RefreshTokenResponseDto
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt
        };

        return new RefreshResult(response, newRefreshToken.PlainToken, newRefreshToken.ExpiresAt);
    }

    public async Task LogoutAsync(string plainRefreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = _jwtTokenService.HashToken(plainRefreshToken);
        var existingToken = await _unitOfWork.RefreshTokens.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (existingToken is not null && existingToken.RevokedAt is null)
        {
            existingToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(existingToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<RefreshTokenResult> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var generated = _jwtTokenService.GenerateRefreshToken();

        var entity = new RefreshToken
        {
            UserId = userId,
            TokenHash = _jwtTokenService.HashToken(generated.PlainToken),
            ExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime)
        };

        await _unitOfWork.RefreshTokens.AddAsync(entity, cancellationToken);

        return generated with { ExpiresAt = entity.ExpiresAt };
    }
}
