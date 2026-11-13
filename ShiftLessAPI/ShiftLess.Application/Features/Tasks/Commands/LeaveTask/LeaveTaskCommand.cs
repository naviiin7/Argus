using MediatR;

namespace ShiftLess.Application.Features.Tasks.Commands.LeaveTask;

public record LeaveTaskCommand(
    int TaskId,
    int WorkerId)
    : IRequest<LeaveTaskResponse>;