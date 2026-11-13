using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Admin.DTOs;

public class UpdateUserRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    /// <summary>"Manager" or "Client" (a worker).</summary>
    public string Role { get; set; } = string.Empty;
}