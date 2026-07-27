using MediatR;
using ShiftLess.Application.Features.Tasks.Responses;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Application.Features.Tasks.DTOs;

namespace ShiftLess.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, CreateTaskResponse>
{
    private readonly ITaskRepository _taskRepository;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<CreateTaskResponse> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = new TaskRequest
        {
            ShopkeeperId = request.ShopkeeperId,
            Title = request.Request.Title,
            Description = request.Request.Description,
            Budget = request.Request.Budget,
            RequiredWorkers = request.Request.RequiredWorkers,

            StartTime = request.Request.StartTime,

            WorkingHours = request.Request.WorkingHours,

            LeaveNoticeHours = request.Request.LeaveNoticeHours,

            Status = ShiftLess.Domain.Enums.TaskStatus.Open
        };

        await _taskRepository.AddAsync(task);

        await _taskRepository.SaveChangesAsync();

        return new CreateTaskResponse
        {
            TaskId = task.Id,
            Message = "Task created successfully."
        };
    }
}