using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Auth.DTOs;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}