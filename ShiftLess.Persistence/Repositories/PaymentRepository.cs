using Microsoft.EntityFrameworkCore;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Persistence.Context;

namespace ShiftLess.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public async Task<Payment?> GetByOrderIdAsync(string orderId)
    {
        return await _context.Payments
            .Include(x => x.TaskRequest)
            .FirstOrDefaultAsync(x => x.RazorpayOrderId == orderId);
    }

    public async Task<Payment?> GetByTaskIdAsync(int taskId)
    {
        return await _context.Payments
            .Where(x => x.TaskRequestId == taskId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}