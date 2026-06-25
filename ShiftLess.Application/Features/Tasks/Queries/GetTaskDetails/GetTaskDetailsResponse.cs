namespace ShiftLess.Application.Features.Tasks.Queries.GetTaskDetails;

public class GetTaskDetailsResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Budget { get; set; }

    public int RequiredWorkers { get; set; }

    public DateTime Deadline { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<ApplicantDto> Applicants { get; set; } = [];
}

public class ApplicantDto
{
    public int ApplicationId { get; set; }

    public int WorkerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}