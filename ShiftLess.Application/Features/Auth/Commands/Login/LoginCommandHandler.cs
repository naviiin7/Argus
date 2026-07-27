using MediatR;
using ShiftLess.Application.Features.Auth.Responses;
using ShiftLess.Application.Interfaces;
using RefreshTokenEntity = ShiftLess.Domain.Entities.RefreshToken;

namespace ShiftLess.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository.GetByEmailAsync(
                request.Request.Email);

        if (user is null)
            throw new UnauthorizedAccessException(
                "Invalid email or password.");

        var validPassword =
            _passwordHasher.Verify(
                request.Request.Password,
                user.PasswordHash);

        if (!validPassword)
            throw new UnauthorizedAccessException(
                "Invalid email or password.");

        var token =
            _jwtService.GenerateToken(user);

        var refreshTokenValue =
            _jwtService.GenerateRefreshToken();

        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = _jwtService.GetRefreshTokenExpiry()
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token,
            RefreshToken = refreshTokenValue
        };
    }
}