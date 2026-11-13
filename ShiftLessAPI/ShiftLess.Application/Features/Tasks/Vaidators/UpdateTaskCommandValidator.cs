using FluentValidation;
using ShiftLess.Application.Features.Tasks.Commands.UpdateTask;

namespace ShiftLess.Application.Features.Tasks.Validators;

public class UpdateTaskCommandValidator
    : AbstractValidator<Features.Tasks.Commands.UpdateTask.UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0);

        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Request.Budget)
            .GreaterThan(0);

        RuleFor(x => x.Request.RequiredWorkers)
            .GreaterThan(0);

        RuleFor(x => x.Request.StartTime)
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.Request.LeaveNoticeHours)
            .GreaterThanOrEqualTo(0);
    }
}