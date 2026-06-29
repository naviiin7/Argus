using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Application.Features.Tasks.Commands.ApplyTask;

public class ApplyTaskCommandHandler
    : IRequestHandler<ApplyTaskCommand, ApplyTaskResponse>
{
    private readonly ITaskRepository _taskRepository;

    public ApplyTaskCommandHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }



    public async Task<ApplyTaskResponse> Handle(
     ApplyTaskCommand request,
     CancellationToken cancellationToken)
    {
        var task =
            await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new Exception("Task not found");

        var existingApplication =
            await _taskRepository.GetExistingApplicationAsync(
                request.TaskId,
                request.WorkerId);

        if (DateTime.UtcNow > task.StartTime)
            throw new Exception("Applications are closed.");

        if (existingApplication is not null)
        {
            if (existingApplication.Status != ApplicationStatus.Rejected)
            {
                throw new Exception(
                    "You have already applied for this task.");
            }

            existingApplication.Status =
                ApplicationStatus.Pending;

            existingApplication.AppliedAt =
                DateTime.UtcNow;

            await _taskRepository.SaveChangesAsync();

            return new ApplyTaskResponse
            {
                ApplicationId = existingApplication.Id,
                Message = "Application resubmitted successfully"
            };
        }

        var application = new TaskApplication
        {
            TaskRequestId = request.TaskId,
            WorkerId = request.WorkerId,
            AppliedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Pending
        };

        await _taskRepository.AddApplicationAsync(application);

        await _taskRepository.SaveChangesAsync();

        return new ApplyTaskResponse
        {
            ApplicationId = application.Id,
            Message = "Application submitted successfully"

        };
    }
}
