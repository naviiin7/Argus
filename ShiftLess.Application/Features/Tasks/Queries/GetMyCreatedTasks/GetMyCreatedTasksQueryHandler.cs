using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Queries.GetMyCreatedTasks;

public class GetMyCreatedTasksQueryHandler
    : IRequestHandler<GetMyCreatedTasksQuery, List<GetMyCreatedTasksResponse>>
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
        var tasks = await _taskRepository.GetTasksByManagerAsync(request.ManagerId);

        if (tasks == null)
            return [];

        var responses = new List<GetMyCreatedTasksResponse>();

        foreach (var task in tasks)
        {
            var acceptedCount =
                await _taskRepository.GetAcceptedWorkerCountAsync(task.Id);

            responses.Add(new GetMyCreatedTasksResponse
            {
                TaskId = task.Id,
                Title = task.Title,
                Description = task.Description,
                Budget = task.Budget,
                RequiredWorkers = task.RequiredWorkers,
                AcceptedWorkers = acceptedCount,
                Deadline = task.StartTime,
                Status = task.Status.ToString()
            });
        }

        return responses;
    }
}