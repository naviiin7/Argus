using MediatR;

namespace ShiftLess.Application.Features.Tasks.Queries.GetOpenTasks;

public record GetOpenTasksQuery(
    int WorkerId)
    : IRequest<List<GetOpenTasksResponse>>;