using MediatR;

namespace ShiftLess.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand : IRequest<DeleteTaskResponse>
{
    public DeleteTaskCommand(int id, int managerId, string role)
    {
        Id = id;
        ManagerId = managerId;
        Role = role;
    }

    public int Id { get; }

    public int ManagerId { get; }

    public string Role { get; }
}