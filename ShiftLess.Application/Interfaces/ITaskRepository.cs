using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(TaskRequest task);

    Task SaveChangesAsync();

    Task<TaskRequest?> GetByIdAsync(int id);

    Task AddApplicationAsync(TaskApplication application);

    Task<TaskApplication?> GetExistingApplicationAsync(    int taskId,    int workerId);

    Task<List<TaskRequest>> GetOpenTasksAsync(int workerId);

    Task<List<TaskApplication>> GetApplicantsAsync(int taskId);

    Task<TaskApplication?> GetApplicationByIdAsync(int applicationId);

    Task<List<TaskApplication>> GetAcceptedTasksAsync(int workerId);

    Task<List<TaskRequest>> GetTasksByManagerAsync(int managerId);

    Task<TaskRequest?> GetTaskDetailsAsync(int taskId);

    Task UpdateTaskAsync(TaskRequest task);

    Task<int> GetAcceptedWorkerCountAsync(int taskId);

    Task<List<TaskApplication>> GetApplicationsByTaskIdAsync(int taskId);

    Task<TaskApplication?> GetAcceptedApplicationAsync(int taskId, int workerId);

    Task DeleteTaskAsync(TaskRequest task);

    Task DeleteApplicationsForTaskAsync(int taskId);



}