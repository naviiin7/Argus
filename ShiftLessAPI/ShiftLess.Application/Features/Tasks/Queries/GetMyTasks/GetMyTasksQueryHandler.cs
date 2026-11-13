using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Queries.GetMyTasks;

public class GetMyTasksQueryHandler
    : IRequestHandler<GetMyTasksQuery, List<GetMyTasksResponse>>
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
            await _taskRepository.GetAcceptedTasksAsync(request.WorkerId);

        if (applications == null)
            return [];

        return applications.Select(application => new GetMyTasksResponse
        {
            TaskId = application.TaskRequest.Id,
            Title = application.TaskRequest.Title,
            Description = application.TaskRequest.Description,
            Budget = application.TaskRequest.Budget,
            RequiredWorkers = application.TaskRequest.RequiredWorkers,
            Deadline = application.TaskRequest.StartTime,
            Status = application.TaskRequest.Status.ToString()
        }).ToList();
    }
}