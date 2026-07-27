using FluentValidation;

namespace ShiftLess.Application.Features.Auth.Validators;

public class RegisterCommandValidator
    : AbstractValidator<Features.Auth.Commands.Register.RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Request.Phone)
            .NotEmpty()
            .Matches(@"^\d{10}$")
            .WithMessage("Phone number must contain exactly 10 digits.");

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Must contain one uppercase letter.")
            .Matches("[a-z]").WithMessage("Must contain one lowercase letter.")
            .Matches("[0-9]").WithMessage("Must contain one number.");
    }
}