using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Application.Common.Exceptions;
namespace ShiftLess.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler
    : IRequestHandler<UpdateTaskCommand, string>
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskCommandHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<string> Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task =
            await _taskRepository.GetByIdAsync(
                request.TaskId);

        if (task is null)
            throw new NotFoundException(
                "Task not found.");

        if (request.Role != "Admin" &&
            task.ShopkeeperId != request.ManagerId)
        {
            throw new ForbiddenException(
                "You do not own this task.");
        }

        if (DateTime.UtcNow >= task.StartTime)
            throw new BadRequestException(
                "A task that has already started cannot be edited.");

        var acceptedCount =
            await _taskRepository.GetAcceptedWorkerCountAsync(task.Id);

        // Prevent shrinking capacity below what's already committed.
        // This is what was producing the "3/1 accepted" display —
        // nothing stopped a manager from dropping RequiredWorkers
        // below the number of workers already accepted.
        if (request.Request.RequiredWorkers < acceptedCount)
        {
            throw new BadRequestException(
                $"Cannot reduce required workers below {acceptedCount} " +
                "already-accepted worker(s). Reject a worker first.");
        }

        task.Title =
            request.Request.Title;

        task.Description =
            request.Request.Description;

        task.Budget =
            request.Request.Budget;

        task.RequiredWorkers =
            request.Request.RequiredWorkers;

        task.StartTime =
            request.Request.StartTime;

        task.WorkingHours =
            request.Request.WorkingHours;

        task.LeaveNoticeHours =
            request.Request.LeaveNoticeHours;

        if (task.Status == ShiftLess.Domain.Enums.TaskStatus.Full &&
            acceptedCount < task.RequiredWorkers)
        {
            task.Status = ShiftLess.Domain.Enums.TaskStatus.Open;
        }
        else if (task.Status == ShiftLess.Domain.Enums.TaskStatus.Open &&
                 acceptedCount > 0 &&
                 acceptedCount >= task.RequiredWorkers)
        {
            task.Status = ShiftLess.Domain.Enums.TaskStatus.Full;
        }

        await _taskRepository.UpdateTaskAsync(task);

        return "Task updated successfully.";
    }
}