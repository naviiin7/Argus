using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Features.Payments.Commands.VerifyPayment;

public class VerifyPaymentCommandHandler
    : IRequestHandler<VerifyPaymentCommand, VerifyPaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IPaymentGateway _paymentGateway;

    public VerifyPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        ITaskRepository taskRepository,
        IPaymentGateway paymentGateway)
    {
        _paymentRepository = paymentRepository;
        _taskRepository = taskRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<VerifyPaymentResponse> Handle(
        VerifyPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(
            request.Request.RazorpayOrderId);

        if (payment is null)
            throw new NotFoundException("Payment order not found.");

        if (request.Role != "Admin" &&
            payment.TaskRequest.ShopkeeperId != request.ManagerId)
            throw new ForbiddenException("You do not own this task.");

        var isValid = _paymentGateway.VerifySignature(
            request.Request.RazorpayOrderId,
            request.Request.RazorpayPaymentId,
            request.Request.RazorpaySignature);

        if (!isValid)
        {
            payment.Status = PaymentStatus.Failed;
            await _paymentRepository.SaveChangesAsync();

            throw new BadRequestException(
                "Payment verification failed. Signature mismatch.");
        }

        payment.Status = PaymentStatus.Paid;
        payment.RazorpayPaymentId = request.Request.RazorpayPaymentId;
        payment.PaidAt = DateTime.UtcNow;

        await _paymentRepository.SaveChangesAsync();

        return new VerifyPaymentResponse
        {
            Message = "Payment verified and recorded successfully."
        };
    }
}