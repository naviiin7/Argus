using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Features.Auth.Otp;
using ShiftLess.Application.Features.Auth.Responses;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ResetPasswordResponse> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var email = EmailNormalizer.Normalize(request.Request.Email);

        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null || !user.IsActive)
            throw new BadRequestException(
                "Unable to reset the password for this account.");

        user.PasswordHash = _passwordHasher.Hash(request.Request.NewPassword);

        await _userRepository.SaveChangesAsync();

        // Anyone holding an old session is signed out.
        await _refreshTokenRepository.RevokeAllForUserAsync(user.Id);

        return new ResetPasswordResponse
        {
            Message = "Password reset successfully. You can now log in."
        };
    }
}