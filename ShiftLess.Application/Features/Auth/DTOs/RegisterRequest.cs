using ShiftLess.Domain.Enums;

namespace ShiftLess.Application.Features.Auth.DTOs;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsBusinessOwner { get; set; }

    public string OtpCode { get; set; } = string.Empty;
}