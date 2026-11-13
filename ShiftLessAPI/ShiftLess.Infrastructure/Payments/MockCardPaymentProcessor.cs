using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Infrastructure.Payments;

/// <summary>
/// Demo-only card processor that behaves like a payment provider's test mode:
/// well-known test numbers produce specific declines, any other valid card
/// succeeds. Nothing is stored, and card numbers are never logged (last 4 only).
/// </summary>
public class MockCardPaymentProcessor : ICardPaymentProcessor
{
    private static readonly Dictionary<string, string> Declines = new()
    {
        ["4000000000000002"] = "Your card was declined.",
        ["4000000000009995"] = "Your card has insufficient funds.",
        ["4000000000000069"] = "Your card has expired.",
        ["4000000000000127"] = "Your card's security code is incorrect.",
        ["4000000000000119"] = "A processing error occurred. Please try again."
    };

    private readonly ILogger<MockCardPaymentProcessor> _logger;

    public MockCardPaymentProcessor(ILogger<MockCardPaymentProcessor> logger)
    {
        _logger = logger;
    }

    public async Task<CardChargeResult> ChargeAsync(
        CardChargeRequest request,
        CancellationToken cancellationToken = default)
    {
        var number = new string(
            (request.CardNumber ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());

        // Malformed input is a typo, not a charge attempt, so it is rejected
        // before anything gets recorded.
        if (string.IsNullOrWhiteSpace(request.CardholderName))
            throw new BadRequestException("Enter the name on the card.");

        if (number.Length is < 13 or > 19 || !PassesLuhn(number))
            throw new BadRequestException("Enter a valid card number.");

        if (request.ExpiryMonth is < 1 or > 12)
            throw new BadRequestException("Enter a valid expiry month.");

        var year = request.ExpiryYear < 100
            ? 2000 + request.ExpiryYear
            : request.ExpiryYear;

        if (year is < 2000 or > 2100)
            throw new BadRequestException("Enter a valid expiry year.");

        // A card is valid through the last day of its expiry month.
        if (DateTime.UtcNow >= new DateTime(year, request.ExpiryMonth, 1).AddMonths(1))
            throw new BadRequestException("This card has expired.");

        if (!Regex.IsMatch(request.Cvv ?? string.Empty, @"^\d{3,4}$"))
            throw new BadRequestException("Enter a valid security code.");

        // Simulate processor latency so the UI's "Processing..." state is visible.
        await Task.Delay(900, cancellationToken);

        var last4 = number[^4..];

        if (Declines.TryGetValue(number, out var reason))
        {
            _logger.LogInformation(
                "[MOCK PAYMENT] Card ending {Last4} declined: {Reason}",
                last4,
                reason);

            return new CardChargeResult
            {
                Success = false,
                FailureReason = reason
            };
        }

        var transactionId = $"mock_txn_{Guid.NewGuid():N}";

        _logger.LogInformation(
            "[MOCK PAYMENT] Card ending {Last4} charged {Amount}. Transaction {TransactionId}",
            last4,
            request.Amount,
            transactionId);

        return new CardChargeResult
        {
            Success = true,
            TransactionId = transactionId
        };
    }

    private static bool PassesLuhn(string digits)
    {
        var sum = 0;
        var alternate = false;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var n = digits[i] - '0';

            if (alternate)
            {
                n *= 2;

                if (n > 9)
                    n -= 9;
            }

            sum += n;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}