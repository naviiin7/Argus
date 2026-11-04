using Microsoft.EntityFrameworkCore;
using ShiftLess.Application.Features.Admin.Queries.GetAdminStats;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Domain.Enums;
using PaymentStatusEnum = ShiftLess.Domain.Enums.PaymentStatus;
using ShiftLess.Persistence.Context;
using TaskStatus = ShiftLess.Domain.Enums.TaskStatus;

namespace ShiftLess.Persistence.Repositories;

public class AdminStatsRepository : IAdminStatsRepository
{
    private readonly ApplicationDbContext _context;

    public AdminStatsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminStatsResponse> GetStatsAsync()
    {
        // ---------- users ----------
        var roleCounts = await _context.Users
            .GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .ToListAsync();

        int UsersWithRole(UserRole role) =>
            roleCounts.FirstOrDefault(x => x.Role == role)?.Count ?? 0;

        var blocked = await _context.Users.CountAsync(u => !u.IsActive);

        // ---------- tasks ----------
        var taskCounts = await _context.TaskRequests
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        int TasksWith(TaskStatus status) =>
            taskCounts.FirstOrDefault(x => x.Status == status)?.Count ?? 0;

        // ---------- applications ----------
        var applicationCounts = await _context.TaskApplications
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        int ApplicationsWith(ApplicationStatus status) =>
            applicationCounts.FirstOrDefault(x => x.Status == status)?.Count ?? 0;

        // ---------- chat + payments ----------
        var messages = await _context.ChatMessages.CountAsync();

        var paid = _context.Payments
    .Where(p => p.Status == PaymentStatus.Paid);

        var paidCount = await paid.CountAsync();

        var paidAmount = await paid
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        return new AdminStatsResponse
        {
            Users = new UserStats
            {
                Total = roleCounts.Sum(x => x.Count),
                Admins = UsersWithRole(UserRole.Admin),
                Managers = UsersWithRole(UserRole.Manager),
                Workers = UsersWithRole(UserRole.Client),
                Blocked = blocked
            },

            Tasks = new TaskStats
            {
                Total = taskCounts.Sum(x => x.Count),
                Open = TasksWith(TaskStatus.Open),
                Assigned = TasksWith(TaskStatus.Full),
                InProgress = TasksWith(TaskStatus.InProgress),
                Completed = TasksWith(TaskStatus.Completed),
                Cancelled = TasksWith(TaskStatus.Cancelled),
                Expired = TasksWith(TaskStatus.Expired)
            },

            Applications = new ApplicationStats
            {
                Total = applicationCounts.Sum(x => x.Count),
                Pending = ApplicationsWith(ApplicationStatus.Pending),
                Accepted = ApplicationsWith(ApplicationStatus.Accepted),
                Rejected = ApplicationsWith(ApplicationStatus.Rejected),
                Withdrawn = ApplicationsWith(ApplicationStatus.Withdrawn)
            },

            TotalChatMessages = messages,
            PaidPayments = paidCount,
            TotalPaidAmount = paidAmount
        };
    }
}