using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Queries.GetOpenTasks;

public class GetOpenTasksQueryHandler
    : IRequestHandler<GetOpenTasksQuery, List<GetOpenTasksResponse>>
{
    private readonly ITaskRepository _taskRepository;

    public GetOpenTasksQueryHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<GetOpenTasksResponse>> Handle(
        GetOpenTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetOpenTasksAsync(request.WorkerId);

        if (tasks == null)
            return [];

        var responses = new List<GetOpenTasksResponse>();

        foreach (var task in tasks)
        {
            var acceptedCount =
                await _taskRepository.GetAcceptedWorkerCountAsync(task.Id);

            responses.Add(new GetOpenTasksResponse
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