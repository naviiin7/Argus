using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Features.Payments.Commands.PayTask;

public class PayTaskCommandHandler
    : IRequestHandler<PayTaskCommand, PayTaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICardPaymentProcessor _processor;

    public PayTaskCommandHandler(
        ITaskRepository taskRepository,
        IPaymentRepository paymentRepository,
        ICardPaymentProcessor processor)
    {
        _taskRepository = taskRepository;
        _paymentRepository = paymentRepository;
        _processor = processor;
    }

    public async Task<PayTaskResponse> Handle(
        PayTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new NotFoundException("Task not found.");

        if (request.Role != "Admin" &&
            task.ShopkeeperId != request.ManagerId)
        {
            throw new ForbiddenException("You do not own this task.");
        }

        if (task.Status != ShiftLess.Domain.Enums.TaskStatus.Completed)
            throw new BadRequestException("Only completed tasks can be paid.");

        var existing = await _paymentRepository.GetByTaskIdAsync(task.Id);

        if (existing is not null && existing.Status == PaymentStatus.Paid)
            throw new BadRequestException("This task has already been paid.");

        var charge = await _processor.ChargeAsync(
            new CardChargeRequest
            {
                Amount = task.Budget,
                CardholderName = request.Request.CardholderName,
                CardNumber = request.Request.CardNumber,
                ExpiryMonth = request.Request.ExpiryMonth,
                ExpiryYear = request.Request.ExpiryYear,
                Cvv = request.Request.Cvv
            },
            cancellationToken);

        // Reuses the existing Payment table. The gateway-specific column names
        // (RazorpayOrderId/RazorpayPaymentId) simply hold the mock references.
        var payment = new Payment
        {
            TaskRequestId = task.Id,
            RazorpayOrderId = $"mock_order_{Guid.NewGuid():N}",
            RazorpayPaymentId = string.IsNullOrEmpty(charge.TransactionId)
                ? null
                : charge.TransactionId,
            Amount = task.Budget,
            Status = charge.Success ? PaymentStatus.Paid : PaymentStatus.Failed,
            PaidAt = charge.Success ? DateTime.UtcNow : null
        };

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        // Failed attempts are recorded first, then reported to the caller.
        if (!charge.Success)
            throw new BadRequestException(charge.FailureReason ?? "Payment failed.");

        return new PayTaskResponse
        {
            Message = "Payment successful.",
            TransactionId = charge.TransactionId,
            Amount = task.Budget
        };
    }
}