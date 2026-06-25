using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Queries.GetMyTasks;

public class GetMyTasksQueryHandler
    : IRequestHandler<
        GetMyTasksQuery,
        List<GetMyTasksResponse>>
{
    private readonly ITaskRepository _taskRepository;

    public GetMyTasksQueryHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<GetMyTasksResponse>> Handle(
        GetMyTasksQuery request,
        CancellationToken cancellationToken)
    {
        var applications =
            await _taskRepository.GetAcceptedTasksAsync(
                request.WorkerId);

        return applications
            .Select(x => new GetMyTasksResponse
            {
                TaskId = x.TaskRequest.Id,
                Title = x.TaskRequest.Title,
                Description = x.TaskRequest.Description,
                Budget = x.TaskRequest.Budget,
                Deadline = x.TaskRequest.Deadline
            })
            .ToList();
    }
}