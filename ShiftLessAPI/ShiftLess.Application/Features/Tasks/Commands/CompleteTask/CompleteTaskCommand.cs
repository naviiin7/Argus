using MediatR;

namespace ShiftLess.Application.Features.Tasks.Commands.CompleteTask;

public class CompleteTaskCommand : IRequest<CompleteTaskResponse>
{
    public CompleteTaskCommand(
        int taskId,
        int managerId,
        string role)
    {
        TaskId = taskId;
        ManagerId = managerId;
        Role = role;
    }

    public int TaskId { get; }

    public int ManagerId { get; }

    public string Role { get; }
}