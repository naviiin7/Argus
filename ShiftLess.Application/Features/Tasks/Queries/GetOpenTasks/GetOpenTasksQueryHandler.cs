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
        var tasks =
            await _taskRepository.GetOpenTasksAsync();

        return tasks.Select(x => new GetOpenTasksResponse
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Budget = x.Budget,
            RequiredWorkers = x.RequiredWorkers,
            Deadline = x.Deadline
        }).ToList();
    }
}