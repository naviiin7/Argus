using System;

namespace ShiftLess.Application.Features.Tasks.Queries.GetTaskDetails;

public class GetTaskDetailsResponse
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public int RequiredWorkers { get; set; }
    public int AcceptedWorkers { get; set; }
    public DateTime StartTime { get; set; }
    public int WorkingHours { get; set; }
    public DateTime EndTime { get; set; }
    public int LeaveNoticeHours { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool CanComplete { get; set; }
}