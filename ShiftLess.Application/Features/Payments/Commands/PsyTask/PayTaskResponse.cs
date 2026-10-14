namespace ShiftLess.Application.Features.Payments.Commands.PayTask;

public class PayTaskResponse
{
    public string Message { get; set; } = string.Empty;

    public string TransactionId { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}