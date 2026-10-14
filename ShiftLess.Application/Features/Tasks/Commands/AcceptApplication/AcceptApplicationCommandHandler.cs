using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;
using ShiftLess.Application.Common.Exceptions;

namespace ShiftLess.Application.Features.Tasks.Commands.AcceptApplication;

public class AcceptApplicationCommandHandler
    : IRequestHandler<AcceptApplicationCommand, AcceptApplicationResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public AcceptApplicationCommandHandler(
        ITaskRepository taskRepository,
        IRealtimeNotifier realtimeNotifier)
    {
        _taskRepository = taskRepository;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<AcceptApplicationResponse> Handle(
        AcceptApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var task =
            await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new NotFoundException(
                "Task not found.");

        if (request.Role != "Admin" &&
            task.ShopkeeperId != request.ShopkeeperId)
        {
            throw new ForbiddenException(
                "You do not own this task.");
        }

        var application =
            await _taskRepository.GetApplicationByIdAsync(
                request.ApplicationId);

        if (application is null)
            throw new NotFoundException(
                "Application not found.");

        if (application.TaskRequestId != request.TaskId)
            throw new BadRequestException(
                "Invalid task.");

        if (application.Status != ApplicationStatus.Pending)
        {
            throw new BadRequestException(
                "Only pending applications can be accepted.");
        }

        if (DateTime.UtcNow >= task.StartTime)
            throw new BadRequestException(
                "This task has already started.");

        var acceptedCount =
            await _taskRepository.GetAcceptedWorkerCountAsync(task.Id);

        if (acceptedCount >= task.RequiredWorkers)
            throw new ConflictException(
                "This task is already full.");

        application.Status = ApplicationStatus.Accepted;

        acceptedCount++;

        if (acceptedCount == task.RequiredWorkers)
        {
            task.Status = Domain.Enums.TaskStatus.Full;
        }

        await _taskRepository.SaveChangesAsync();

        await _realtimeNotifier.NotifyUserAsync(
            application.WorkerId,
            "ApplicationAccepted",
            new { taskId = task.Id, taskTitle = task.Title });

        return new AcceptApplicationResponse
        {
            Message = "Application accepted successfully."
        };
    }
}