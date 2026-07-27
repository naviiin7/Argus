using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Queries.GetTaskDetails;

public class GetTaskDetailsQueryHandler
    : IRequestHandler<GetTaskDetailsQuery, GetTaskDetailsResponse>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskDetailsQueryHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<GetTaskDetailsResponse> Handle(
        GetTaskDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var task =
            await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new NotFoundException("Task not found.");

        if (request.Role != "Admin" &&
            task.ShopkeeperId != request.ManagerId)
        {
            throw new ForbiddenException(
                "You do not own this task.");
        }

        var acceptedCount =
            await _taskRepository.GetAcceptedWorkerCountAsync(task.Id);

        var endTime = task.StartTime.AddHours(task.WorkingHours);

        var canComplete =
            task.Status == ShiftLess.Domain.Enums.TaskStatus.InProgress &&
            DateTime.UtcNow >= endTime;

        return new GetTaskDetailsResponse
        {
            TaskId = task.Id,
            Title = task.Title,
            Description = task.Description,
            Budget = task.Budget,
            RequiredWorkers = task.RequiredWorkers,
            AcceptedWorkers = acceptedCount,
            StartTime = task.StartTime,
            WorkingHours = task.WorkingHours,
            EndTime = endTime,
            LeaveNoticeHours = task.LeaveNoticeHours,
            Status = task.Status.ToString(),
            CanComplete = canComplete
        };
    }
}