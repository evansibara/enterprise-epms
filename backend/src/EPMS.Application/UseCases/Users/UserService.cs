using EPMS.Application.DTOs.Common;
using EPMS.Application.DTOs.Users;
using EPMS.Application.Interfaces.Repositories;
using EPMS.Application.Interfaces.Services;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Exceptions;

namespace EPMS.Application.UseCases.Users;

public interface IUserService
{
    Task<UserResponseDto> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default);

    Task<UserResponseDto> UpdateAsync(Guid userId, UpdateUserRequestDto request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserResponseDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<PagedResult<UserResponseDto>> ListAsync(
        string? search, int page, int pageSize, CancellationToken cancellationToken = default);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("Email sudah terdaftar.");
        }

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
        {
            throw new DomainValidationException($"Role '{request.Role}' tidak valid.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = role
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<UserResponseDto> UpdateAsync(
        Guid userId, UpdateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
        {
            throw new DomainValidationException($"Role '{request.Role}' tidak valid.");
        }

        user.Name = request.Name;
        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        _unitOfWork.Users.SoftDelete(user);
        await _unitOfWork.RefreshTokens.RevokeAllForUserAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserResponseDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        return MapToDto(user);
    }

    public async Task<PagedResult<UserResponseDto>> ListAsync(
        string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Users.ListAsync(search, page, pageSize, cancellationToken);
        var dtos = items.Select(MapToDto).ToList();

        return PagedResult<UserResponseDto>.Create(dtos, totalCount, page, pageSize);
    }

    private static UserResponseDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role.ToString(),
        CreatedAt = user.CreatedAt
    };
}
