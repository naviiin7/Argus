using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;
using ShiftLess.Application.Common.Exceptions;
namespace ShiftLess.Application.Features.Tasks.Commands.LeaveTask;

public class LeaveTaskCommandHandler
    : IRequestHandler<LeaveTaskCommand, LeaveTaskResponse>
{
    private readonly ITaskRepository _taskRepository;

    public LeaveTaskCommandHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<LeaveTaskResponse> Handle(
        LeaveTaskCommand request,
        CancellationToken cancellationToken)
    {
        var application =
            await _taskRepository.GetAcceptedApplicationAsync(
                request.TaskId,
                request.WorkerId);

        if (application is null)
            throw new BadRequestException(
                "You are not assigned to this task.");

        var task = application.TaskRequest;

        if (task.Status == Domain.Enums.TaskStatus.Completed)
            throw new BadRequestException(
                "Completed tasks cannot be left.");

        var leaveDeadline =
            task.StartTime.AddHours(-task.LeaveNoticeHours);

        if (DateTime.UtcNow > leaveDeadline)
            throw new BadRequestException(
                $"Workers must leave at least {task.LeaveNoticeHours} hours before the task starts.");

        application.Status = ApplicationStatus.Withdrawn;

        var acceptedWorkers =
            (await _taskRepository.GetApplicationsByTaskIdAsync(task.Id))
            .Count(x => x.Status == ApplicationStatus.Accepted);

        if (acceptedWorkers < task.RequiredWorkers)
        {
            task.Status = Domain.Enums.TaskStatus.Open;
        }

        await _taskRepository.SaveChangesAsync();

        return new LeaveTaskResponse
        {
            Message = "You have left the task successfully."
        };
    }
}