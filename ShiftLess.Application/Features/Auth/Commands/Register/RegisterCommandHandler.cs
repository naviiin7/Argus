using MediatR;
using ShiftLess.Application.Features.Auth.Responses;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> Handle(
    RegisterCommand request,
    CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository
            .GetByEmailAsync(request.Request.Email);

        if (existingUser is not null)
        {
            throw new Exception("Email already exists.");
        }

        var user = new User
        {
            FullName = request.Request.FullName,
            Email = request.Request.Email,
            Phone = request.Request.Phone,
            PasswordHash = _passwordHasher.Hash(
                request.Request.Password),

            Role = request.Request.IsBusinessOwner? UserRole.Manager : UserRole.Client,
                        KycStatus = KycStatus.Pending,

                        IsActive = true
        };

        await _userRepository.AddAsync(user);

        await _userRepository.SaveChangesAsync();

        return new RegisterResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Message = "Registration successful."
        };
    }
}