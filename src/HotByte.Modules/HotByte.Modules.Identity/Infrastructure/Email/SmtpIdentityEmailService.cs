using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HotByte.Modules.Identity.Application.Interfaces;

namespace HotByte.Modules.Identity.Infrastructure.Email
{
    public class SmtpIdentityEmailService : IIdentityEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpIdentityEmailService> _logger;

        public SmtpIdentityEmailService(IConfiguration config, ILogger<SmtpIdentityEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail)) return;

            var host = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
            var portString = _config["Email:SmtpPort"] ?? "587";
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var fromAddress = _config["Email:FromAddress"] ?? username;
            var fromName = _config["Email:FromName"] ?? "HotByte";

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromAddress))
            {
                _logger.LogWarning("Email credentials not configured. Skipping email to {To}.", toEmail);
                return;
            }

            if (!int.TryParse(portString, out int port)) port = 587;

            try
            {
                using var smtp = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(fromAddress, fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                await smtp.SendMailAsync(message);
                _logger.LogInformation("Email sent to {To} with subject '{Subject}'", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", toEmail);
            }
        }
    }
}
