using FluentValidation;
using ShiftLess.Application.Features.Auth.Otp;

namespace ShiftLess.Application.Features.Auth.Validators;

public class ResetPasswordCommandValidator
    : AbstractValidator<Features.Auth.Commands.ResetPassword.ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .Must(e => EmailNormalizer.IsValid(EmailNormalizer.Normalize(e)))
            .WithMessage("Enter a valid email address.");

        RuleFor(x => x.Request.OtpCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Enter the verification code.")
            .Matches(@"^\d{6}$").WithMessage("The verification code must be 6 digits.");

        RuleFor(x => x.Request.NewPassword)
            .MustBeStrongPassword();
    }
}