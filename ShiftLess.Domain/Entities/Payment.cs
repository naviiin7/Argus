using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Domain.Entities;

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2
}

public class Payment
{
    public int Id { get; set; }

    public int TaskRequestId { get; set; }
    public TaskRequest TaskRequest { get; set; } = null!;

    public string RazorpayOrderId { get; set; } = string.Empty;

    public string? RazorpayPaymentId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PaidAt { get; set; }
}