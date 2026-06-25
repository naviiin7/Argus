using MediatR;

namespace ShiftLess.Application.Features.Tasks.Queries.GetMyCreatedTasks;

public record GetMyCreatedTasksQuery(
    int ManagerId)
    : IRequest<List<GetMyCreatedTasksResponse>>;