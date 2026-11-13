using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Infrastructure.Email;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var text = Regex.Replace(htmlBody, "<[^>]+>", " ");

        text = Regex.Replace(text, @"\s+", " ").Trim();

        _logger.LogWarning(
            "[DEMO EMAIL] To: {To} | Subject: {Subject} | {Body}",
            toEmail,
            subject,
            text);

        return Task.CompletedTask;
    }
}