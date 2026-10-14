namespace ShiftLess.Infrastructure.Email;

public class EmailSettings
{
    // Leave Host empty to run in demo mode (emails are logged, not sent).
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public bool EnableSsl { get; set; } = true;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FromAddress { get; set; } = string.Empty;

    public string FromName { get; set; } = "ShiftLess";
}