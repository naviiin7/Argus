using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler
    : IRequestHandler<DeleteTaskCommand, DeleteTaskResponse>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<DeleteTaskResponse> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task =
            await _taskRepository.GetByIdAsync(request.Id);

        if (task is null)
            throw new Exception("Task not found.");

        if (task.ShopkeeperId != request.ManagerId)
            throw new UnauthorizedAccessException(
                "You do not own this task.");

        await _taskRepository.DeleteApplicationsForTaskAsync(task.Id);

        await _taskRepository.DeleteTaskAsync(task);

        await _taskRepository.SaveChangesAsync();

        return new DeleteTaskResponse
        {
            Message = "Task deleted successfully."
        };
    }
}