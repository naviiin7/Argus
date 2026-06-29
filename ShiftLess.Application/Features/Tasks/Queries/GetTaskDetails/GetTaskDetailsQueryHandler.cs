using MediatR;
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
        var task = await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new Exception("Task not found.");

        if (request.Role != "Admin" &&
    task.ShopkeeperId != request.ManagerId)
        {
            throw new UnauthorizedAccessException(
                "You do not own this task.");
        }

        var applications =
            await _taskRepository.GetApplicationsByTaskIdAsync(task.Id);

        return new GetTaskDetailsResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Budget = task.Budget,
            RequiredWorkers = task.RequiredWorkers,
            Deadline = task.StartTime,
            Status = task.Status.ToString(),

            Applicants = applications.Select(a => new ApplicantDto
            {
                ApplicationId = a.Id,
                WorkerId = a.WorkerId,
                Name = a.Worker.FullName,
                Email = a.Worker.Email,
                Status = a.Status.ToString()
            }).ToList()
        };
    }
}