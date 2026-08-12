using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MyCRM.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration; //field stores the configuration settings -> User Secrets

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var sender = _configuration["Email:Sender"] ?? "astanislawska128@gmail.com";
            string password = _configuration["Email:Password"] ?? throw new InvalidOperationException("Email password is not configured.");

            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            int smtpPort = int.TryParse(_configuration["Email:SmtpPort"], out var p) ? p : 587;

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(sender),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(sender, password)
            };

            await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
        }
    }
}
