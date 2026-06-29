using MediatR;
using ShiftLess.Application.Features.Tasks.DTOs;

public record UpdateTaskCommand(
    int TaskId,
    int ManagerId,
    string Role,
    UpdateTaskRequest Request)
    : IRequest<string>;