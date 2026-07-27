using FluentValidation;

namespace ShiftLess.Application.Features.Tasks.Validators;

public class ApplyTaskCommandValidator
    : AbstractValidator<Features.Tasks.Commands.ApplyTask.ApplyTaskCommand>
{
    public ApplyTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0);

        RuleFor(x => x.WorkerId)
            .GreaterThan(0);
    }
}