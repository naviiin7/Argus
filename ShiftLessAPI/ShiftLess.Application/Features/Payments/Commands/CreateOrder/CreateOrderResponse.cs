using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Payments.Commands.CreateOrder;

public class CreateOrderResponse
{
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayKeyId { get; set; } = string.Empty;
    public int AmountInPaise { get; set; }
    public string Currency { get; set; } = "INR";
}