using System;

namespace ShiftLess.Application.Features.Tasks.Queries.GetMyCreatedTasks;

public class GetMyCreatedTasksResponse
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public int RequiredWorkers { get; set; }
    public int AcceptedWorkers { get; set; }
    public DateTime Deadline { get; set; }
    public string Status { get; set; } = string.Empty;
}