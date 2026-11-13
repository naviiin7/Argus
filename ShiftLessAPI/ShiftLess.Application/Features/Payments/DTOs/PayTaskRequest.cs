using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Payments.DTOs;

public class PayTaskRequest
{
    public string CardholderName { get; set; } = string.Empty;

    public string CardNumber { get; set; } = string.Empty;

    public int ExpiryMonth { get; set; }

    public int ExpiryYear { get; set; }

    public string Cvv { get; set; } = string.Empty;
}