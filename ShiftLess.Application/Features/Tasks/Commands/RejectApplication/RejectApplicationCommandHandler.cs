using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;
using ShiftLess.Application.Common.Exceptions;
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
            throw new NotFoundException(
                "Application not found.");

        if (application.TaskRequestId != request.TaskId)
            throw new BadRequestException(
                "Invalid task.");

        if (application.Status != ApplicationStatus.Pending)
        {
            throw new BadRequestException(
                "Only pending applications can be rejected.");
        }

        if (request.Role != "Admin")
        {
            var task =
                await _taskRepository.GetByIdAsync(request.TaskId);

            if (task is null)
                throw new NotFoundException(
                    "Task not found.");

            if (task.ShopkeeperId != request.ShopkeeperId)
                throw new ForbiddenException(
                    "You do not own this task.");
        }

        application.Status = ApplicationStatus.Rejected;

        await _taskRepository.SaveChangesAsync();

        return new RejectApplicationResponse
        {
            Message = "Application rejected successfully."
        };
    }
}