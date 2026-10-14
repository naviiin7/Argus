using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Payments.Commands.CreateOrder;

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;

    public CreateOrderCommandHandler(
        ITaskRepository taskRepository,
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway)
    {
        _taskRepository = taskRepository;
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<CreateOrderResponse> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new NotFoundException("Task not found.");

        if (request.Role != "Admin" && task.ShopkeeperId != request.ManagerId)
            throw new ForbiddenException("You do not own this task.");

        if (task.Status != ShiftLess.Domain.Enums.TaskStatus.Completed)
            throw new BadRequestException(
                "Only completed tasks can be paid out.");

        var existing = await _paymentRepository.GetByTaskIdAsync(task.Id);

        if (existing is not null && existing.Status == Domain.Entities.PaymentStatus.Paid)
            throw new BadRequestException("This task has already been paid.");

        var order = await _paymentGateway.CreateOrderAsync(
            task.Budget,
            receipt: $"task-{task.Id}");

        var payment = new Domain.Entities.Payment
        {
            TaskRequestId = task.Id,
            RazorpayOrderId = order.OrderId,
            Amount = task.Budget,
            Status = Domain.Entities.PaymentStatus.Pending
        };

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return new CreateOrderResponse
        {
            RazorpayOrderId = order.OrderId,
            RazorpayKeyId = _paymentGateway.GetPublicKeyId(),
            AmountInPaise = order.AmountInPaise,
            Currency = order.Currency
        };
    }
}