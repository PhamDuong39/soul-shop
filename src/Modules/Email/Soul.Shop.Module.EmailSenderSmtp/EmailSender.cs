using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Services;

namespace Soul.Shop.Module.EmailSenderSmtp;

public class EmailSender(
    IRepository<EmailSend> emilSendRepository,
    ILogger<EmailSender> logger,
    IOptionsMonitor<EmailSmtpOptions> options)
    : IEmailSender
{
    private readonly EmailSmtpOptions _options = options.CurrentValue;

    public async Task SendEmailAsync(string email, string subject, string body, bool isHtml = false)
    {
        var send = new EmailSend();
        try
        {
            if (string.IsNullOrWhiteSpace(_options.SmtpUserName))
                throw new ArgumentNullException(nameof(_options.SmtpUserName));
            if (string.IsNullOrWhiteSpace(_options.SmtpPassword))
                throw new ArgumentNullException(nameof(_options.SmtpPassword));

            send.From = _options.SmtpUserName;
            send.To = email;
            send.Subject = subject;
            send.IsHtml = isHtml;
            send.Body = body;

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_options.SmtpUserName));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var textFormat = isHtml ? TextFormat.Html : TextFormat.Plain;
            message.Body = new TextPart(textFormat)
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_options.SmtpHost, _options.SmtpPort, SecureSocketOptions.StartTls);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(_options.SmtpUserName, _options.SmtpPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            send.IsSucceed = true;
            logger.LogInformation("Mail sent successfully");
        }
        catch (Exception ex)
        {
            send.Message = ex.Message;
            send.IsSucceed = false;
            logger.LogError(ex, "Email sending exception");
        }
        finally
        {
            emilSendRepository.Add(send);
            await emilSendRepository.SaveChangesAsync();
        }
    }
}