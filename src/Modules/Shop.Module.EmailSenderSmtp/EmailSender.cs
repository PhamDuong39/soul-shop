// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Services;

namespace Shop.Module.EmailSenderSmtp;

public class EmailSender : IEmailSender
{
    private readonly EmailSmtpOptions _options;
    private readonly IRepository<EmailSend> _emilSendRepository;
    private readonly ILogger<EmailSender> _logger

    public EmailSender(
        IRepository<EmailSend> emilSendRepository,
        ILogger<EmailSender> logger,
        IOptionsMonitor<EmailSmtpOptions> options)
    {
        _options = options.CurrentValue;
        _emilSendRepository = emilSendRepository;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string email, string subject, string body, bool isHtml = false)
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
            _logger.LogInformation("邮件发送成功", send);
            return true;
        }
        catch (Exception ex)
        {
            send.Message = ex.Message;
            send.IsSucceed = false;
            _logger.LogError(ex, "邮件发送异常", send, email, subject, body, isHtml);
            return false;
        }
        finally
        {
            _emilSendRepository.Add(send);
            await _emilSendRepository.SaveChangesAsync();
        }
    }
}
