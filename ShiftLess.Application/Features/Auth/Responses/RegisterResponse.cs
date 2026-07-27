using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Auth.Responses;

public class RegisterResponse
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}