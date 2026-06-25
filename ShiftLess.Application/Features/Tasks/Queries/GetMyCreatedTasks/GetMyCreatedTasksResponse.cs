using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Tasks.Queries.GetMyCreatedTasks;

public class GetMyCreatedTasksResponse
{
    public int TaskId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int RequiredWorkers { get; set; }

    public DateTime Deadline { get; set; }

    public string Status { get; set; } = string.Empty;
}