using MediatR;
using ShiftLess.Application.Features.Tasks.DTOs;
using ShiftLess.Application.Features.Tasks.Responses;

namespace ShiftLess.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    int ShopkeeperId,
    CreateTaskRequest Request)
    : IRequest<CreateTaskResponse>;