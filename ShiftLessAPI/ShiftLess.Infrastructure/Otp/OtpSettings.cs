using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Infrastructure.Otp;

public class OtpSettings
{
    public string Pepper { get; set; } = string.Empty;
    public bool ExposeCodeInResponse { get; set; }

}