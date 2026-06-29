using ShiftLess.Domain.Enums;

namespace ShiftLess.Domain.Entities;

public class TaskRequest
{
    public int Id { get; set; }

    public int ShopkeeperId { get; set; }
    public User Shopkeeper { get; set; } = null!;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Budget { get; set; }

    public int RequiredWorkers { get; set; }

    // Scheduled start time
    public DateTime StartTime { get; set; }

    public int LeaveNoticeHours { get; set; }

    public Domain.Enums.TaskStatus Status { get; set; }
}