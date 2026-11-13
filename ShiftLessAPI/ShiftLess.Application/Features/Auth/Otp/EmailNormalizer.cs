using System.Net.Mail;

namespace ShiftLess.Application.Features.Auth.Otp;

public static class EmailNormalizer
{
    public static string Normalize(string? email)
    {
        return string.IsNullOrWhiteSpace(email)
            ? string.Empty
            : email.Trim().ToLowerInvariant();
    }

    public static bool IsValid(string normalized)
    {
        if (string.IsNullOrEmpty(normalized) || normalized.Length > 254)
            return false;

        try
        {
            var address = new MailAddress(normalized);

            // Rejects display-name forms like "Name <a@b.com>" and
            // domains without a dot.
            return address.Address == normalized &&
                   address.Host.Contains('.');
        }
        catch (FormatException)
        {
            return false;
        }
    }
}