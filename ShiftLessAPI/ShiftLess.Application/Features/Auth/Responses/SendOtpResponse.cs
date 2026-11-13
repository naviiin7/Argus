namespace ShiftLess.Application.Features.Auth.Responses;

public class SendOtpResponse
{
    public int ExpiresInSeconds { get; set; }

    public int ResendAfterSeconds { get; set; }

    // Only populated when Otp:ExposeCodeInResponse is true (dev/demo).
    public string? DebugCode { get; set; }
}