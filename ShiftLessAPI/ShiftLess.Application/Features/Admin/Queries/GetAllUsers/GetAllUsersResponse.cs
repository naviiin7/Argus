namespace ShiftLess.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersResponse
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    /// <summary>True when the user has tasks, applications or messages (so they can only be blocked, not deleted).</summary>
    public bool HasActivity { get; set; }
}