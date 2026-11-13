using MediatR;
using ShiftLess.Application.Features.Payments.DTOs;

namespace ShiftLess.Application.Features.Payments.Commands.PayTask;

public record PayTaskCommand(
    int TaskId,
    int ManagerId,
    string Role,
    PayTaskRequest Request)
    : IRequest<PayTaskResponse>;