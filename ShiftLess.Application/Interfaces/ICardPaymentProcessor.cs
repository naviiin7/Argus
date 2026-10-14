using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Interfaces;

public class CardChargeRequest
{
    public decimal Amount { get; set; }

    public string CardholderName { get; set; } = string.Empty;

    public string CardNumber { get; set; } = string.Empty;

    public int ExpiryMonth { get; set; }

    public int ExpiryYear { get; set; }

    public string Cvv { get; set; } = string.Empty;
}

public class CardChargeResult
{
    public bool Success { get; set; }

    public string TransactionId { get; set; } = string.Empty;

    public string? FailureReason { get; set; }
}

public interface ICardPaymentProcessor
{
    Task<CardChargeResult> ChargeAsync(
        CardChargeRequest request,
        CancellationToken cancellationToken = default);
}