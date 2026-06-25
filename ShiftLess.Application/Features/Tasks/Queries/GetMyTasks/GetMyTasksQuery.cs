using MediatR;

namespace ShiftLess.Application.Features.Tasks.Queries.GetMyTasks;

public record GetMyTasksQuery(
    int WorkerId)
    : IRequest<List<GetMyTasksResponse>>;