using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Features.Auth.Otp;
using ShiftLess.Application.Features.Auth.Responses;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Features.Auth.Commands.SendOtp;

public class SendOtpCommandHandler
    : IRequestHandler<SendOtpCommand, SendOtpResponse>
{
    private readonly IOtpRepository _otpRepository;
    private readonly IOtpService _otpService;
    private readonly IEmailSender _emailSender;
    private readonly IUserRepository _userRepository;

    public SendOtpCommandHandler(
        IOtpRepository otpRepository,
        IOtpService otpService,
        IEmailSender emailSender,
        IUserRepository userRepository)
    {
        _otpRepository = otpRepository;
        _otpService = otpService;
        _emailSender = emailSender;
        _userRepository = userRepository;
    }

    public async Task<SendOtpResponse> Handle(
        SendOtpCommand request,
        CancellationToken cancellationToken)
    {
        var email = EmailNormalizer.Normalize(request.Email);

        if (!EmailNormalizer.IsValid(email))
            throw new BadRequestException("Enter a valid email address.");

        if (request.Purpose != OtpPurpose.Registration &&
            request.Purpose != OtpPurpose.PasswordReset)
            throw new BadRequestException("Unsupported verification purpose.");

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (request.Purpose == OtpPurpose.Registration && existingUser is not null)
            throw new ConflictException(
                "An account with this email already exists. Try logging in instead.");

        // Password reset: reply exactly the same whether or not the account
        // exists, so this endpoint can't be used to discover registered emails.
        if (request.Purpose == OtpPurpose.PasswordReset &&
            (existingUser is null || !existingUser.IsActive))
        {
            return BuildResponse(null);
        }

        var latest = await _otpRepository.GetLatestAsync(email, request.Purpose);

        if (latest is not null)
        {
            var elapsed = (DateTime.UtcNow - latest.CreatedAt).TotalSeconds;

            var remaining = OtpPolicy.ResendCooldownSeconds - (int)elapsed;

            if (remaining > 0)
                throw new BadRequestException(
                    $"Please wait {remaining} seconds before requesting another code.");
        }

        await _otpRepository.InvalidateActiveAsync(email, request.Purpose);

        var code = _otpService.GenerateCode();

        await _otpRepository.AddAsync(new OtpCode
        {
            Target = email,
            Purpose = request.Purpose,
            CodeHash = _otpService.Hash(email, request.Purpose, code),
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpPolicy.ExpiryMinutes)
        });

        var (subject, body) = BuildEmail(request.Purpose, code);

        // Send BEFORE saving: if delivery fails, nothing is persisted, so the
        // user isn't locked behind the resend cooldown for an email that
        // never arrived.
        await _emailSender.SendAsync(email, subject, body, cancellationToken);

        await _otpRepository.SaveChangesAsync();

        return BuildResponse(_otpService.ExposeDebugCode ? code : null);
    }

    private static SendOtpResponse BuildResponse(string? debugCode)
    {
        return new SendOtpResponse
        {
            ExpiresInSeconds = OtpPolicy.ExpiryMinutes * 60,
            ResendAfterSeconds = OtpPolicy.ResendCooldownSeconds,
            DebugCode = debugCode
        };
    }

    private static (string Subject, string Body) BuildEmail(string purpose, string code)
    {
        var isReset = purpose == OtpPurpose.PasswordReset;

        var subject = isReset
            ? "Reset your ShiftLess password"
            : "Your ShiftLess verification code";

        var heading = isReset ? "Reset your password" : "Verify your email";

        var intro = isReset
            ? "Use this code to reset your ShiftLess password:"
            : "Use this code to finish creating your ShiftLess account:";

        var body =
            "<div style='font-family:Arial,sans-serif;max-width:420px'>" +
            $"<h2 style='margin:0 0 12px'>{heading}</h2>" +
            $"<p>{intro}</p>" +
            $"<p style='font-size:32px;font-weight:700;letter-spacing:6px;margin:16px 0'>{code}</p>" +
            $"<p style='color:#666'>It expires in {OtpPolicy.ExpiryMinutes} minutes. " +
            "If you didn't request this, you can ignore this email.</p>" +
            "</div>";

        return (subject, body);
    }
}