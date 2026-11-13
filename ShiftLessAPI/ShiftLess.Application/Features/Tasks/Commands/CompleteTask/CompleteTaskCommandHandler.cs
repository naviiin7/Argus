using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;
using ShiftLess.Application.Common.Exceptions;
namespace ShiftLess.Application.Features.Tasks.Commands.CompleteTask;

public class CompleteTaskCommandHandler
    : IRequestHandler<CompleteTaskCommand, CompleteTaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public CompleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IRealtimeNotifier realtimeNotifier)
    {
        _taskRepository = taskRepository;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<CompleteTaskResponse> Handle(
        CompleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task =
            await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new NotFoundException(
                "Task not found.");

        if (request.Role != "Admin" &&
            task.ShopkeeperId != request.ManagerId)
        {
            throw new ForbiddenException(
                "You do not own this task.");
        }

        if (task.Status != ShiftLess.Domain.Enums.TaskStatus.InProgress)
            throw new BadRequestException(
                "Only tasks that are in progress can be completed.");

        var endTime = task.StartTime.AddHours(task.WorkingHours);

        if (DateTime.UtcNow < endTime)
            throw new BadRequestException(
                $"This task's working hours don't end until " +
                $"{endTime:yyyy-MM-dd HH:mm} UTC. It can only be marked " +
                "complete after that time.");

        task.Status = ShiftLess.Domain.Enums.TaskStatus.Completed;

        await _taskRepository.SaveChangesAsync();

        var applications =
            await _taskRepository.GetApplicationsByTaskIdAsync(task.Id);

        foreach (var app in applications.Where(a => a.Status == ApplicationStatus.Accepted))
        {
            await _realtimeNotifier.NotifyUserAsync(
                app.WorkerId,
                "TaskCompleted",
                new { taskId = task.Id, taskTitle = task.Title });
        }

        return new CompleteTaskResponse
        {
            Message = "Task completed successfully."
        };
    }
}