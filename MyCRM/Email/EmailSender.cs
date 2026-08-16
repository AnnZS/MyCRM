using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace MyCRM.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var senderAddress = "astanislawska128@gmail.com"; // fixed sender as requested
            var senderName = _configuration["Email:SenderName"] ?? "MyCRM";
            var password = _configuration["Email:Password"] ?? throw new InvalidOperationException("Email password is not configured.");

            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_configuration["Email:SmtpPort"], out var p) ? p : 587;

            var messageBuilder = new MimeMessage();
            messageBuilder.From.Add(new MailboxAddress(senderName, senderAddress));
            messageBuilder.To.Add(MailboxAddress.Parse(email));
            messageBuilder.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            messageBuilder.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                // Connect with STARTTLS if available
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls).ConfigureAwait(false);

                // Authenticate using the configured sender and password
                await client.AuthenticateAsync(senderAddress, password).ConfigureAwait(false);

                await client.SendAsync(messageBuilder).ConfigureAwait(false);
            }
            finally
            {
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
