using FluentValidation;
using ShiftLess.Application.Features.Admin.Commands.UpdateUsers;

namespace ShiftLess.Application.Features.Admin.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Request.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must be 100 characters or fewer.");

        RuleFor(x => x.Request.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{10,15}$")
            .WithMessage("Phone number must be 10 to 15 digits, optionally starting with +.");

        RuleFor(x => x.Request.Role)
            .NotEmpty().WithMessage("Role is required.");
    }
}