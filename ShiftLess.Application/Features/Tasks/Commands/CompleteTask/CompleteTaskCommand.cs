using MediatR;

namespace ShiftLess.Application.Features.Tasks.Commands.CompleteTask;

public class CompleteTaskCommand : IRequest<CompleteTaskResponse>
{
    public CompleteTaskCommand(int taskId, int managerId)
    {
        TaskId = taskId;
        ManagerId = managerId;
    }

    public int TaskId { get; }

    public int ManagerId { get; }
}