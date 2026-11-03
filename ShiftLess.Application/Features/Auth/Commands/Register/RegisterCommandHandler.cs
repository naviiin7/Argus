using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Features.Auth.Otp;
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
        // Same normalization the OTP step uses, so "Name@Mail.com " and
        // "name@mail.com" are the same account.
        var email = EmailNormalizer.Normalize(request.Request.Email);

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null)
            throw new ConflictException("Email already exists.");

        var user = new User
        {
            FullName = request.Request.FullName.Trim(),
            Email = email,
            Phone = request.Request.Phone.Trim(),
            PasswordHash = _passwordHasher.Hash(request.Request.Password),

            Role = request.Request.IsBusinessOwner
                ? UserRole.Manager
                : UserRole.Client,

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
        };
    }
}