using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Application.Features.Tasks.Commands.AcceptApplication;

public class AcceptApplicationCommandHandler
    : IRequestHandler<
        AcceptApplicationCommand,
        AcceptApplicationResponse>
{
    private readonly ITaskRepository _taskRepository;

    public AcceptApplicationCommandHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<AcceptApplicationResponse> Handle(
        AcceptApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var task =
            await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new Exception("Task not found");

        var application =
    await _taskRepository.GetApplicationByIdAsync(request.ApplicationId);

        if (application is null)
            throw new Exception("Application not found");

            await _taskRepository.GetApplicationByIdAsync(
                request.ApplicationId);

        if (application is null)
            throw new Exception("Application not found");

        if (application.TaskRequestId != request.TaskId)
            throw new Exception("Invalid task");

        // Already accepted?
        if (application.Status == ApplicationStatus.Accepted)
            throw new Exception("Application is already accepted.");

        // Check available slots
        var acceptedCount =
            await _taskRepository.GetAcceptedWorkerCountAsync(task.Id);

        if (acceptedCount >= task.RequiredWorkers)
            throw new Exception("This task is already full.");

        // Accept worker
        application.Status = ApplicationStatus.Accepted;

        acceptedCount++;

        // If task is now full, mark it Assigned
        if (acceptedCount == task.RequiredWorkers)
        {
            task.Status = Domain.Enums.TaskStatus.Full;
        }

        if (DateTime.UtcNow >= task.StartTime)
            throw new Exception("This task has already started.");

        await _taskRepository.SaveChangesAsync();

        return new AcceptApplicationResponse
        {
            Message = "Application accepted successfully."
        };
    }
}