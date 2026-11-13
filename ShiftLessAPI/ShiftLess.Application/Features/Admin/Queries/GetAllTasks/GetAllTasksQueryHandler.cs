using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Admin.Queries.GetAllTasks;

public class GetAllTasksQueryHandler
    : IRequestHandler<GetAllTasksQuery, List<GetAllTasksResponse>>
{
    private readonly ITaskRepository _taskRepository;

    public GetAllTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<GetAllTasksResponse>> Handle(
        GetAllTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllTasksAsync();

        var responses = new List<GetAllTasksResponse>();

        foreach (var task in tasks)
        {
            var acceptedCount =
                await _taskRepository.GetAcceptedWorkerCountAsync(task.Id);

            responses.Add(new GetAllTasksResponse
            {
                TaskId = task.Id,
                Title = task.Title,
                ManagerName = task.Shopkeeper?.FullName ?? "Unknown",
                Budget = task.Budget,
                RequiredWorkers = task.RequiredWorkers,
                AcceptedWorkers = acceptedCount,
                StartTime = task.StartTime,
                Status = task.Status.ToString()
            });
        }

        return responses;
    }
}