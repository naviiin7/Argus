using MediatR;
using ShiftLess.Application.Features.Auth.Responses;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByEmailAsync(request.Request.Email);

        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        var validPassword =
            _passwordHasher.Verify(
                request.Request.Password,
                user.PasswordHash);

        if (!validPassword)
        {
            throw new Exception("Invalid email or password.");
        }

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        };
    }
}