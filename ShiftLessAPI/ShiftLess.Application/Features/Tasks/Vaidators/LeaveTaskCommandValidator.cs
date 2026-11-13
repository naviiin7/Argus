using FluentValidation;

namespace ShiftLess.Application.Features.Tasks.Validators;

public class LeaveTaskCommandValidator
    : AbstractValidator<Features.Tasks.Commands.LeaveTask.LeaveTaskCommand>
{
    public LeaveTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0);

        RuleFor(x => x.WorkerId)
            .GreaterThan(0);
    }
}