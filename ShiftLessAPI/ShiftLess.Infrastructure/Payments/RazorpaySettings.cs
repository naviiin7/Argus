using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Infrastructure.Payments;

public class RazorpaySettings
{
    public string KeyId { get; set; } = string.Empty;

    public string KeySecret { get; set; } = string.Empty;
}