using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace ShiftLess.Application.Features.Payments.Commands.CreateOrder;

public record CreateOrderCommand(
    int TaskId,
    int ManagerId,
    string Role)
    : IRequest<CreateOrderResponse>;