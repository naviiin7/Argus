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
    await _taskRepository.GetByIdAsync(
        request.TaskId);

        if (task is null)
            throw new Exception("Task not found");

        if (task.ShopkeeperId != request.ShopkeeperId)
        {
            throw new UnauthorizedAccessException(
                "You do not own this task.");
        }


        var application =
            await _taskRepository.GetApplicationByIdAsync(
                request.ApplicationId);

        if (application is null)
            throw new Exception("Application not found");

        if (application.TaskRequestId != request.TaskId)
            throw new Exception("Invalid task");

        application.Status =
            ApplicationStatus.Accepted;

        await _taskRepository.SaveChangesAsync();

        return new AcceptApplicationResponse
        {
            Message = "Application accepted successfully"
        };
    }
}