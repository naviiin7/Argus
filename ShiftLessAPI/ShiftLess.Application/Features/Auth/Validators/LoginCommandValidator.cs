using FluentValidation;
using ShiftLess.Application.Features.Auth.Otp;

namespace ShiftLess.Application.Features.Auth.Validators;

public class LoginCommandValidator
    : AbstractValidator<Features.Auth.Commands.Login.LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .Must(e => EmailNormalizer.IsValid(EmailNormalizer.Normalize(e)))
            .WithMessage("Invalid email address.");

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}