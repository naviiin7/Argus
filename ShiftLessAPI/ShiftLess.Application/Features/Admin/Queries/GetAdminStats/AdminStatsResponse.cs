using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Admin.Queries.GetAdminStats;

public class AdminStatsResponse
{
    public UserStats Users { get; set; } = new();

    public TaskStats Tasks { get; set; } = new();

    public ApplicationStats Applications { get; set; } = new();

    public int TotalChatMessages { get; set; }

    public int PaidPayments { get; set; }

    public decimal TotalPaidAmount { get; set; }
}

public class UserStats
{
    public int Total { get; set; }

    public int Admins { get; set; }

    public int Managers { get; set; }

    public int Workers { get; set; }

    public int Blocked { get; set; }
}

public class TaskStats
{
    public int Total { get; set; }

    public int Open { get; set; }

    public int Assigned { get; set; }

    public int InProgress { get; set; }

    public int Completed { get; set; }

    public int Cancelled { get; set; }

    public int Expired { get; set; }
}

public class ApplicationStats
{
    public int Total { get; set; }

    public int Pending { get; set; }

    public int Accepted { get; set; }

    public int Rejected { get; set; }

    public int Withdrawn { get; set; }
}