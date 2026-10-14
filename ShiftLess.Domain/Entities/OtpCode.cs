namespace ShiftLess.Domain.Entities;

public class OtpCode
{
    public int Id { get; set; }

    // The address the code was sent to (normalized email).
    public string Target { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    // Only a keyed hash is stored, never the code itself.
    public string CodeHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    public int Attempts { get; set; }

    public DateTime? ConsumedAt { get; set; }

    public bool IsActive =>
        ConsumedAt is null && DateTime.UtcNow < ExpiresAt;
}