using System;
using System.Collections.Generic;
using System.Text;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Domain.Entities
{
    public class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public KycStatus KycStatus { get; set; }

    public bool IsActive { get; set; } = true;
}
}
