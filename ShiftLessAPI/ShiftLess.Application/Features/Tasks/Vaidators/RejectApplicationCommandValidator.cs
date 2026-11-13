using FluentValidation;

namespace ShiftLess.Application.Features.Tasks.Validators;

public class RejectApplicationCommandValidator
    : AbstractValidator<Features.Tasks.Commands.RejectApplication.RejectApplicationCommand>
{
    public RejectApplicationCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0);

        RuleFor(x => x.ApplicationId)
            .GreaterThan(0);

        RuleFor(x => x.ShopkeeperId)
            .GreaterThan(0);
    }
}