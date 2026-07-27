using MediatR;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Domain.Enums;
using ShiftLess.Application.Common.Exceptions;


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
            throw new NotFoundException(
                "Task not found.");

        if (task.Status != ShiftLess.Domain.Enums.TaskStatus.Open)
        {
            throw new BadRequestException(
                "This task is no longer accepting applications.");
        }


        if (DateTime.UtcNow > task.StartTime)
            throw new BadRequestException(
                "Applications are closed.");

        var existingApplication =
            await _taskRepository.GetExistingApplicationAsync(
                request.TaskId,
                request.WorkerId);

        if (existingApplication is not null)
        {
            // A worker can re-apply if their previous application ended
            // in Rejected OR Withdrawn (left the task themselves). Only
            // Pending/Accepted should block a fresh application.
            var canReapply =
                existingApplication.Status == ApplicationStatus.Rejected ||
                existingApplication.Status == ApplicationStatus.Withdrawn;

            if (!canReapply)
            {
                throw new ConflictException(
                    "You have already applied for this task.");
            }

            existingApplication.Status =
                ApplicationStatus.Pending;

            existingApplication.AppliedAt =
                DateTime.UtcNow;

            await _taskRepository.SaveChangesAsync();

            return new ApplyTaskResponse
            {
                ApplicationId = existingApplication.TaskApplicationId,
                Message = "Application resubmitted successfully."
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
            ApplicationId = application.TaskApplicationId,
            Message = "Application submitted successfully."
        };
    }
}