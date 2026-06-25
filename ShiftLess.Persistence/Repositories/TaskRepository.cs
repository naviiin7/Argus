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
        return await _context.TaskRequests
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddApplicationAsync(TaskApplication application)
    {
        await _context.TaskApplications.AddAsync(application);
    }

    public async Task<List<TaskApplication>> GetApplicantsAsync(int taskId)
    {
        return await _context.TaskApplications
            .Include(x => x.Worker)
            .Where(x => x.TaskRequestId == taskId)
            .ToListAsync();
    }

    public async Task<List<TaskRequest>> GetOpenTasksAsync()
    {
        return await _context.TaskRequests
            .Where(x => x.Status == ShiftLess.Domain.Enums.TaskStatus.Open)
            .ToListAsync();
    }

    public async Task<TaskApplication?> GetApplicationByIdAsync(
    int applicationId)
    {
        return await _context.TaskApplications
            .Include(x => x.TaskRequest)
            .FirstOrDefaultAsync(x => x.Id == applicationId);
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

    public async Task<TaskApplication?> GetExistingApplicationAsync(
    int taskId,
    int workerId)
    {
        return await _context.TaskApplications
            .FirstOrDefaultAsync(x =>
                x.TaskRequestId == taskId &&
                x.WorkerId == workerId);
    }

    public async Task<List<TaskRequest>> GetTasksByManagerAsync(int managerId)
    {
        return await _context.TaskRequests
            .Where(x => x.ShopkeeperId == managerId)
            .OrderByDescending(x => x.Created)
            .ToListAsync();
    }

    public async Task<TaskRequest?> GetTaskDetailsAsync(int taskId)
    {
        return await _context.TaskRequests
            .FirstOrDefaultAsync(x => x.Id == taskId);
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

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}