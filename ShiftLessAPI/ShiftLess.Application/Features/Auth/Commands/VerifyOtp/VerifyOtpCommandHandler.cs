using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Features.Auth.Otp;
using ShiftLess.Application.Features.Auth.Responses;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Auth.Commands.VerifyOtp;

public class VerifyOtpCommandHandler
    : IRequestHandler<VerifyOtpCommand, VerifyOtpResponse>
{
    private readonly IOtpRepository _otpRepository;
    private readonly IOtpService _otpService;

    public VerifyOtpCommandHandler(
        IOtpRepository otpRepository,
        IOtpService otpService)
    {
        _otpRepository = otpRepository;
        _otpService = otpService;
    }

    public async Task<VerifyOtpResponse> Handle(
        VerifyOtpCommand request,
        CancellationToken cancellationToken)
    {
        var email = EmailNormalizer.Normalize(request.Email);

        var code = (request.Code ?? string.Empty).Trim();

        if (code.Length == 0)
            throw new BadRequestException(
                "Enter the verification code sent to your email.");

        var otp = await _otpRepository.GetLatestActiveAsync(email, request.Purpose);

        if (otp is null)
            throw new BadRequestException(
                "No active verification code for this email. Please request a new one.");

        var submittedHash = _otpService.Hash(email, request.Purpose, code);

        if (!_otpService.HashesMatch(submittedHash, otp.CodeHash))
        {
            otp.Attempts++;

            var exhausted = otp.Attempts >= OtpPolicy.MaxAttempts;

            if (exhausted)
                otp.ConsumedAt = DateTime.UtcNow;

            await _otpRepository.SaveChangesAsync();

            throw new BadRequestException(
                exhausted
                    ? "Too many incorrect attempts. Please request a new code."
                    : "Incorrect verification code.");
        }

        if (request.Consume)
        {
            otp.ConsumedAt = DateTime.UtcNow;

            await _otpRepository.SaveChangesAsync();
        }

        return new VerifyOtpResponse { Verified = true };
    }
}