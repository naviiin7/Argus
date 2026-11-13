using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Application.Common.Exceptions;
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
            throw new NotFoundException(
                "Task not found.");

        if (request.Role != "Admin" &&
            task.ShopkeeperId != request.ManagerId)
        {
            throw new ForbiddenException(
                "You do not own this task.");
        }

        await _taskRepository.DeleteApplicationsForTaskAsync(task.Id);

        await _taskRepository.DeleteTaskAsync(task);

        await _taskRepository.SaveChangesAsync();

        return new DeleteTaskResponse
        {
            Message = "Task deleted successfully."
        };
    }
}