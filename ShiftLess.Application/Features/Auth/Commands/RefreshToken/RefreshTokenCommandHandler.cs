using MediatR;
using ShiftLess.Application.Features.Auth.Responses;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var existing =
            await _refreshTokenRepository.GetByTokenAsync(
                request.Request.RefreshToken);

        if (existing is null || !existing.IsActive)
            throw new UnauthorizedAccessException(
                "Invalid or expired refresh token.");

        var user =
            await _userRepository.GetByIdAsync(existing.UserId);

        if (user is null)
            throw new UnauthorizedAccessException(
                "Invalid refresh token.");

        var newAccessToken =
            _jwtService.GenerateToken(user);

        var newRefreshTokenValue =
            _jwtService.GenerateRefreshToken();

        existing.RevokedAt = DateTime.UtcNow;
        existing.ReplacedByToken = newRefreshTokenValue;

        var newRefreshToken = new ShiftLess.Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAt = _jwtService.GetRefreshTokenExpiry()
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = newAccessToken,
            RefreshToken = newRefreshTokenValue
        };
    }
}