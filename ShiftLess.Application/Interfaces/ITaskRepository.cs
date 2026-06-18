using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(TaskRequest task);

    Task SaveChangesAsync();
}