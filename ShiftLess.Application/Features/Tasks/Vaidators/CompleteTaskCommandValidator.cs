using FluentValidation;

namespace ShiftLess.Application.Features.Tasks.Validators;

public class CompleteTaskCommandValidator
    : AbstractValidator<Features.Tasks.Commands.CompleteTask.CompleteTaskCommand>
{
    public CompleteTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0);

        RuleFor(x => x.ManagerId)
            .GreaterThan(0);
    }
}