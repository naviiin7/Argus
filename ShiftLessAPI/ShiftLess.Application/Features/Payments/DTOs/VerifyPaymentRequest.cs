using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Payments.DTOs;

public class VerifyPaymentRequest
{
    public string RazorpayOrderId { get; set; } = string.Empty;
    public string RazorpayPaymentId { get; set; } = string.Empty;
    public string RazorpaySignature { get; set; } = string.Empty;
}