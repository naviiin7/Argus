using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Auth.DTOs;

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;

    public string OtpCode { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;
}