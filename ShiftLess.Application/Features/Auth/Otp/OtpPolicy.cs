using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Auth.Otp;

public static class OtpPurpose
{
    public const string Registration = "Registration";
}

public static class OtpPolicy
{
    public const int ExpiryMinutes = 5;

    public const int MaxAttempts = 5;

    public const int ResendCooldownSeconds = 30;
}