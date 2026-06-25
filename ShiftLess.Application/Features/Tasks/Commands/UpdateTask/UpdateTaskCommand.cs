using MediatR;
using ShiftLess.Application.Features.Tasks.DTOs;

public record UpdateTaskCommand(
    int TaskId,
    int ManagerId,
    UpdateTaskRequest Request)
    : IRequest<string>;