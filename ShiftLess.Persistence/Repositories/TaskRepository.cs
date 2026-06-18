using Microsoft.EntityFrameworkCore;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
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

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}