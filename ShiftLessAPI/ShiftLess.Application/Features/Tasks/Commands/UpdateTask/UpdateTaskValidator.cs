using FluentValidation;

namespace ShiftLess.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskValidator()
    {
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

        RuleFor(x => x.Request.LeaveNoticeHours)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Request.StartTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Start time must be in the future.");
    }
}