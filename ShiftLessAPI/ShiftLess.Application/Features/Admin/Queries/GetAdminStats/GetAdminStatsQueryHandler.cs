using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Admin.Queries.GetAdminStats;

public class GetAdminStatsQueryHandler
    : IRequestHandler<GetAdminStatsQuery, AdminStatsResponse>
{
    private readonly IAdminStatsRepository _statsRepository;
    private readonly ITaskRepository _taskRepository;

    public GetAdminStatsQueryHandler(
        IAdminStatsRepository statsRepository,
        ITaskRepository taskRepository)
    {
        _statsRepository = statsRepository;
        _taskRepository = taskRepository;
    }

    public async Task<AdminStatsResponse> Handle(
        GetAdminStatsQuery request,
        CancellationToken cancellationToken)
    {
        // Task statuses move forward with time (Open -> Expired, Full ->
        // InProgress) and are updated whenever tasks are read. Reading them
        // first makes the numbers current.
        await _taskRepository.GetAllTasksAsync();

        return await _statsRepository.GetStatsAsync();
    }
}

