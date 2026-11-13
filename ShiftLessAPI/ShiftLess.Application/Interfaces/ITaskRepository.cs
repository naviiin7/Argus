using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(TaskRequest task);

    Task<TaskRequest?> GetByIdAsync(int id);

    Task AddApplicationAsync(TaskApplication application);

    Task<List<TaskApplication>> GetApplicantsAsync(int taskId);

    Task<List<TaskRequest>> GetOpenTasksAsync(int workerId);

    Task<List<TaskRequest>> GetAllTasksAsync();

    Task<TaskApplication?> GetApplicationByIdAsync(int applicationId);

    Task<List<TaskApplication>> GetAcceptedTasksAsync(int workerId);

    Task<List<TaskRequest>> GetTasksByManagerAsync(int managerId);

    Task<TaskRequest?> GetTaskDetailsAsync(int taskId);

    Task UpdateTaskAsync(TaskRequest task);

    Task<List<TaskApplication>> GetApplicationsByTaskIdAsync(int taskId);

    Task<TaskApplication?> GetAcceptedApplicationAsync(int taskId, int workerId);

    Task<int> GetAcceptedWorkerCountAsync(int taskId);

    Task<List<int>> GetAcceptedWorkerIdsAsync(int taskId);

    Task DeleteTaskAsync(TaskRequest task);

    Task DeleteApplicationsForTaskAsync(int taskId);

    Task<TaskApplication?> GetExistingApplicationAsync(int taskId, int workerId);

    Task SaveChangesAsync();
}