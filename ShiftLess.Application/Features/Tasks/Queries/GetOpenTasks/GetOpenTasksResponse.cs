using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Tasks.Queries.GetOpenTasks;

public class GetOpenTasksResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Budget { get; set; }

    public int RequiredWorkers { get; set; }

    public DateTime Deadline { get; set; }
}