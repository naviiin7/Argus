using MediatR;

namespace ShiftLess.Application.Features.Tasks.Queries.GetApplicants;

public record GetApplicantsQuery(int TaskId)
    : IRequest<List<GetApplicantsResponse>>;