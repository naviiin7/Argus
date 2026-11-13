using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);

    Task<Payment?> GetByOrderIdAsync(string orderId);

    Task<Payment?> GetByTaskIdAsync(int taskId);

    Task SaveChangesAsync();
}