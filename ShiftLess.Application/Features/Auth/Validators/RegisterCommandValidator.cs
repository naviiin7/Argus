using FluentValidation;
using ShiftLess.Application.Features.Auth.Otp;

namespace ShiftLess.Application.Features.Auth.Validators;

public class RegisterCommandValidator
    : AbstractValidator<Features.Auth.Commands.Register.RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must be 100 characters or fewer.");

        RuleFor(x => x.Request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .Must(e => EmailNormalizer.IsValid(EmailNormalizer.Normalize(e)))
            .WithMessage("Enter a valid email address.");

        RuleFor(x => x.Request.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{10,15}$")
            .WithMessage("Phone number must be 10 to 15 digits, optionally starting with +.");

        RuleFor(x => x.Request.Password)
            .MustBeStrongPassword();
    }
}