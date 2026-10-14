using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Interfaces;

public class RazorpayOrderResult
{
    public string OrderId { get; set; } = string.Empty;
    public string Currency { get; set; } = "INR";
    public int AmountInPaise { get; set; }
}

public interface IPaymentGateway
{
    Task<RazorpayOrderResult> CreateOrderAsync(decimal amount, string receipt);

    bool VerifySignature(string orderId, string paymentId, string signature);

    string GetPublicKeyId();
}