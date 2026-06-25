using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Queries.GetApplicants;

public class GetApplicantsQueryHandler
    : IRequestHandler<GetApplicantsQuery, List<GetApplicantsResponse>>
{
    private readonly ITaskRepository _taskRepository;

    public GetApplicantsQueryHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<GetApplicantsResponse>> Handle(
        GetApplicantsQuery request,
        CancellationToken cancellationToken)
    {
        var applicants =
            await _taskRepository.GetApplicantsAsync(request.TaskId);


        return applicants.Select(x => new GetApplicantsResponse
        {
            ApplicationId = x.Id,
            WorkerId = x.WorkerId,
            Name = x.Worker.FullName,
            Email = x.Worker.Email,
            Status = x.Status.ToString()
        }).ToList();


    }
}