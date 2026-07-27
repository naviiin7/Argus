using System;

namespace ShiftLess.Application.Features.Tasks.DTOs;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Budget { get; set; }

    public int WorkingHours { get; set; }

    public int RequiredWorkers { get; set; }

    public DateTime StartTime { get; set; }

    public int LeaveNoticeHours { get; set; } = 24;
}