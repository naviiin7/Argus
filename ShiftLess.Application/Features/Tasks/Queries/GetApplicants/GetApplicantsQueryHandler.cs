using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Tasks.Queries.GetApplicants;

public class GetApplicantsQueryHandler
    : IRequestHandler<GetApplicantsQuery, List<ApplicantResponse>>
{
    private readonly ITaskRepository _taskRepository;

    public GetApplicantsQueryHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<ApplicantResponse>> Handle(
        GetApplicantsQuery request,
        CancellationToken cancellationToken)
    {
        var applicants =
            await _taskRepository.GetApplicantsAsync(request.TaskId);

        return applicants
            .Select(a => new ApplicantResponse
            {
                ApplicationId = a.TaskApplicationId,
                WorkerId = a.WorkerId,
                FullName = a.Worker.FullName,
                Email = a.Worker.Email,
                Status = a.Status.ToString()
            })
            .ToList();
    }
}