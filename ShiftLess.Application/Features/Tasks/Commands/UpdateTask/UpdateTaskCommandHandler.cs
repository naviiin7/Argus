using MediatR;
using ShiftLess.Application.Interfaces;

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
            await _taskRepository
                .GetByIdAsync(request.TaskId);

        if (task is null)
            throw new Exception("Task not found");

        if (task.ShopkeeperId != request.ManagerId)
            throw new UnauthorizedAccessException(
                "You do not own this task.");

        task.Title =
            request.Request.Title;

        task.Description =
            request.Request.Description;

        task.Budget =
            request.Request.Budget;

        task.RequiredWorkers =
            request.Request.RequiredWorkers;

        task.Deadline =
            request.Request.Deadline;

        await _taskRepository
            .UpdateTaskAsync(task);

        return "Task updated successfully";
    }
}