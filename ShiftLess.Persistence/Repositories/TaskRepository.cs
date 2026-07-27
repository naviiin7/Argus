using Microsoft.EntityFrameworkCore;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Domain.Enums;
using ShiftLess.Persistence.Context;

namespace ShiftLess.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskRequest task)
    {
        await _context.TaskRequests.AddAsync(task);
    }

    public async Task<TaskRequest?> GetByIdAsync(int id)
    {
        var task = await _context.TaskRequests
            .FirstOrDefaultAsync(x => x.Id == id);
        if (task != null)
        {
            UpdateTaskStatus(task);
            await _context.SaveChangesAsync();
        }

        return task;
    }

    public async Task AddApplicationAsync(TaskApplication application)
    {
        await _context.TaskApplications.AddAsync(application);
    }

    public async Task<List<TaskApplication>> GetApplicantsAsync(int taskId)
    {
        await GetByIdAsync(taskId);

        return await _context.TaskApplications
            .Include(x => x.Worker)
            .Where(x => x.TaskRequestId == taskId)
            .ToListAsync();
    }

    public async Task<List<TaskRequest>> GetOpenTasksAsync(int workerId)
    {
        var tasks = await _context.TaskRequests.ToListAsync();

        foreach (var task in tasks)
        {
            UpdateTaskStatus(task);
        }

        await _context.SaveChangesAsync();

        // Public listing now includes Full tasks too (shown as "Assigned"
        // with a fill count on the frontend) instead of hiding them the
        // moment they're fully staffed.
        return await _context.TaskRequests
            .Where(t =>
                t.Status == ShiftLess.Domain.Enums.TaskStatus.Open ||
                t.Status == ShiftLess.Domain.Enums.TaskStatus.Full)
            .Where(t =>
                !_context.TaskApplications.Any(a =>
                    a.TaskRequestId == t.Id &&
                    a.WorkerId == workerId &&
                    a.Status != ApplicationStatus.Rejected &&
                    a.Status != ApplicationStatus.Withdrawn))
            .OrderBy(t => t.StartTime)
            .ToListAsync();
    }

    public async Task<TaskApplication?> GetApplicationByIdAsync(
    int applicationId)
    {
        return await _context.TaskApplications
            .Include(x => x.TaskRequest)
            .FirstOrDefaultAsync(x => x.TaskApplicationId == applicationId);
    }

    public async Task<List<TaskApplication>> GetAcceptedTasksAsync(
    int workerId)
    {
        return await _context.TaskApplications
            .Include(x => x.TaskRequest)
            .Where(x =>
                x.WorkerId == workerId &&
                x.Status == ApplicationStatus.Accepted)
            .ToListAsync();
    }


    public async Task<List<TaskRequest>> GetTasksByManagerAsync(int managerId)
    {
        var tasks = await _context.TaskRequests
            .Where(x => x.ShopkeeperId == managerId)
            .OrderByDescending(x => x.Created)
            .ToListAsync();

        foreach (var task in tasks)
        {
            UpdateTaskStatus(task);
        }

        await _context.SaveChangesAsync();

        return tasks;
    }

    public async Task<TaskRequest?> GetTaskDetailsAsync(int taskId)
    {
        var task = await _context.TaskRequests
     .FirstOrDefaultAsync(x => x.Id == taskId);

        if (task != null)
        {
            UpdateTaskStatus(task);
            await _context.SaveChangesAsync();
        }

        return task;
    }

    public async Task UpdateTaskAsync(TaskRequest task)
    {
        _context.TaskRequests.Update(task);

        await _context.SaveChangesAsync();
    }

    public async Task<List<TaskApplication>> GetApplicationsByTaskIdAsync(int taskId)
    {
        return await _context.TaskApplications
            .Where(x => x.TaskRequestId == taskId)
            .Include(x => x.Worker)
            .ToListAsync();
    }

    public async Task<TaskApplication?> GetAcceptedApplicationAsync(
    int taskId,
    int workerId)
    {
        return await _context.TaskApplications
            .Include(x => x.TaskRequest)
            .FirstOrDefaultAsync(x =>
                x.TaskRequestId == taskId &&
                x.WorkerId == workerId &&
                x.Status == ApplicationStatus.Accepted);
    }


    public async Task<int> GetAcceptedWorkerCountAsync(int taskId)
    {
        return await _context.TaskApplications
            .CountAsync(x =>
                x.TaskRequestId == taskId &&
                x.Status == ApplicationStatus.Accepted);
    }

    public Task DeleteTaskAsync(TaskRequest task)
    {
        _context.TaskRequests.Remove(task);
        return Task.CompletedTask;
    }

    public async Task DeleteApplicationsForTaskAsync(int taskId)
    {
        var applications = await _context.TaskApplications
            .Where(x => x.TaskRequestId == taskId)
            .ToListAsync();

        _context.TaskApplications.RemoveRange(applications);
    }

    private void UpdateTaskStatus(TaskRequest task)
    {
        var now = DateTime.UtcNow;

        if (task.Status == ShiftLess.Domain.Enums.TaskStatus.Completed ||
            task.Status == ShiftLess.Domain.Enums.TaskStatus.Cancelled ||
            task.Status == ShiftLess.Domain.Enums.TaskStatus.Expired)
            return;

        if (task.StartTime <= now)
        {
            if (task.Status == ShiftLess.Domain.Enums.TaskStatus.Full)
            {
                task.Status = ShiftLess.Domain.Enums.TaskStatus.InProgress;
            }
            else if (task.Status == ShiftLess.Domain.Enums.TaskStatus.Open)
            {
                task.Status = ShiftLess.Domain.Enums.TaskStatus.Expired;
            }
        }
    }

    public async Task<TaskApplication?> GetExistingApplicationAsync(
    int taskId,
    int workerId)
    {
        return await _context.TaskApplications
            .FirstOrDefaultAsync(x =>
                x.TaskRequestId == taskId &&
                x.WorkerId == workerId);
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}