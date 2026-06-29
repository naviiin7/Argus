using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;
using TaskStatus = ShiftLess.Domain.Enums.TaskStatus;
namespace ShiftLess.Application.Features.Tasks.Commands.CompleteTask;

public class CompleteTaskCommandHandler
    : IRequestHandler<CompleteTaskCommand, CompleteTaskResponse>
{
    private readonly ITaskRepository _taskRepository;

    public CompleteTaskCommandHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<CompleteTaskResponse> Handle(
        CompleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task =
            await _taskRepository.GetByIdAsync(request.TaskId);

        if (task == null)
            throw new Exception("Task not found.");

        if (request.Role != "Admin" &&
    task.ShopkeeperId != request.ManagerId)
        {
            throw new UnauthorizedAccessException(
                "You do not own this task.");
        }

        if (task.Status != TaskStatus.InProgress)
            throw new Exception(
                "Only tasks that are in progress can be completed.");

        task.Status = TaskStatus.Completed;

        await _taskRepository.SaveChangesAsync();

        return new CompleteTaskResponse
        {
            Message = "Task completed successfully."
        };
    }
}