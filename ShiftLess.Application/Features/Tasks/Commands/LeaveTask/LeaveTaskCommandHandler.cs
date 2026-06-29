using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;

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
            throw new Exception(
                "You are not assigned to this task.");

        var task = application.TaskRequest;

        var leaveDeadline =
            task.StartTime.AddHours(-task.LeaveNoticeHours);

        if (DateTime.UtcNow > leaveDeadline)
            throw new Exception(
                $"Workers must leave at least {task.LeaveNoticeHours} hours before the task deadline.");

        application.Status = ApplicationStatus.Withdrawn;

        var acceptedWorkers =
            (await _taskRepository.GetApplicationsByTaskIdAsync(task.Id))
            .Count(x => x.Status == ApplicationStatus.Accepted);

        if (acceptedWorkers < task.RequiredWorkers)
            task.Status =Domain.Enums.TaskStatus.Open;

        await _taskRepository.SaveChangesAsync();

        return new LeaveTaskResponse
        {
            Message = "You have left the task successfully."
        };
    }
}