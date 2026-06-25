using MediatR;

namespace ShiftLess.Application.Features.Tasks.Queries.GetOpenTasks;

public record GetOpenTasksQuery()
    : IRequest<List<GetOpenTasksResponse>>;