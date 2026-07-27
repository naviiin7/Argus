using FluentValidation;

namespace ShiftLess.Application.Features.Tasks.Validators;

public class CreateTaskCommandValidator
    : AbstractValidator<Features.Tasks.Commands.CreateTask.CreateTaskCommand>
{
    public CreateTaskCommandValidator()
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

        RuleFor(x => x.Request.StartTime)
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.Request.WorkingHours)
            .GreaterThan(0)
            .WithMessage("Working hours must be greater than 0.");

        RuleFor(x => x.Request.LeaveNoticeHours)
            .GreaterThanOrEqualTo(0);
    }
}