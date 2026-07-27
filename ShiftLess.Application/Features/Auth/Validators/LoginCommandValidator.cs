using FluentValidation;

namespace ShiftLess.Application.Features.Auth.Validators;

public class LoginCommandValidator
    : AbstractValidator<Features.Auth.Commands.Login.LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email address.");

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}