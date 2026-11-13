using MediatR;
using ShiftLess.Application.Features.Payments.DTOs;

namespace ShiftLess.Application.Features.Payments.Commands.VerifyPayment;

public record VerifyPaymentCommand(
    int ManagerId,
    string Role,
    VerifyPaymentRequest Request)
    : IRequest<VerifyPaymentResponse>;