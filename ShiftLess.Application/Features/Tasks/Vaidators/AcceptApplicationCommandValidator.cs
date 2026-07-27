using FluentValidation;

namespace ShiftLess.Application.Features.Tasks.Validators;

public class AcceptApplicationCommandValidator
    : AbstractValidator<Features.Tasks.Commands.AcceptApplication.AcceptApplicationCommand>
{
    public AcceptApplicationCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0);

        RuleFor(x => x.ApplicationId)
            .GreaterThan(0);

        RuleFor(x => x.ShopkeeperId)
            .GreaterThan(0);
    }
}