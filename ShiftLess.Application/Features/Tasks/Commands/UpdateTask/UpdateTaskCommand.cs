using MediatR;
using ShiftLess.Application.Features.Tasks.DTOs;

namespace ShiftLess.Application.Features.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand(
    int TaskId,
    int ManagerId,
    string Role,
    UpdateTaskRequest Request)
    : IRequest<string>;