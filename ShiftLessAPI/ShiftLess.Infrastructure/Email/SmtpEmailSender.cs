using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(
        IOptions<EmailSettings> options,
        ILogger<SmtpEmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var fromAddress = string.IsNullOrWhiteSpace(_settings.FromAddress)
            ? _settings.Username
            : _settings.FromAddress;

        using var message = new MailMessage
        {
            From = new MailAddress(fromAddress, _settings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 15000
        };

        try
        {
            await client.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // The real cause (bad credentials, blocked port...) goes to the
            // server log; the user only gets a safe, actionable message.
            _logger.LogError(ex, "Failed to send email to {To}", toEmail);

            throw new BadRequestException(
                "We couldn't send the verification email. Please check the address and try again.");
        }
    }
}