using MediatR;

namespace ShiftLess.Application.Features.Tasks.Queries.GetTaskDetails;

public record GetTaskDetailsQuery(
    int TaskId,
    int ManagerId,
    string Role)
    : IRequest<GetTaskDetailsResponse>;