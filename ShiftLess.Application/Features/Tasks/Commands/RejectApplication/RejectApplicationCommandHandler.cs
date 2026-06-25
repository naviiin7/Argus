using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Application.Features.Tasks.Commands.RejectApplication;

public class RejectApplicationCommandHandler
    : IRequestHandler<
        RejectApplicationCommand,
        RejectApplicationResponse>
{
    private readonly ITaskRepository _taskRepository;

    public RejectApplicationCommandHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<RejectApplicationResponse> Handle(
        RejectApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var application =
            await _taskRepository.GetApplicationByIdAsync(
                request.ApplicationId);

        if (application is null)
            throw new Exception("Application not found");

        if (application.TaskRequestId != request.TaskId)
            throw new Exception("Invalid task");

        application.Status = ApplicationStatus.Rejected;

        await _taskRepository.SaveChangesAsync();

        return new RejectApplicationResponse
        {
            Message = "Application rejected successfully"
        };
    }
}