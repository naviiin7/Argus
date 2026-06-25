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

    public DateTime Deadline { get; set; }

    public int MinimumLeaveNoticeHours { get; set; } = 24;

    public DateTime ApplicationDeadline { get; set; }

    public ShiftLess.Domain.Enums.TaskStatus Status { get; set; }
}