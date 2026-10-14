using System;

namespace ShiftLess.Application.Features.Admin.Queries.GetAllTasks;

public class GetAllTasksResponse
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public int RequiredWorkers { get; set; }
    public int AcceptedWorkers { get; set; }
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = string.Empty;
}