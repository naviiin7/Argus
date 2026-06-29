using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Queries.GetMyCreatedTasks;

public class GetMyCreatedTasksQueryHandler
    : IRequestHandler<
        GetMyCreatedTasksQuery,
        List<GetMyCreatedTasksResponse>>
{
    private readonly ITaskRepository _taskRepository;

    public GetMyCreatedTasksQueryHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<GetMyCreatedTasksResponse>> Handle(
        GetMyCreatedTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks =
            await _taskRepository
                .GetTasksByManagerAsync(request.ManagerId);

        return tasks.Select(x =>
            new GetMyCreatedTasksResponse
            {
                TaskId = x.Id,
                Title = x.Title,
                RequiredWorkers = x.RequiredWorkers,
                Deadline = x.StartTime,
                Status = x.Status.ToString()
            })
            .ToList();
    }
}