using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace TicketEmailWorker.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmail(
            string fromEmail,
            string toEmail,
            string subject,
            string cCEmail,
            string body,
            CancellationToken cancellationToken)
        {
            var smtpServer = _configuration["SmtpSettings:Server"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
            var smtpUsername = _configuration["SmtpSettings:Username"];
            var smtpPassword = _configuration["SmtpSettings:Password"];
            var bccEmail = _configuration["FlightAdmin:BccEmail"];

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            };

            var msg = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            msg.To.Add(toEmail);

            if (!string.IsNullOrWhiteSpace(cCEmail))
                msg.CC.Add(cCEmail);

            if (!string.IsNullOrWhiteSpace(bccEmail))
                msg.Bcc.Add(bccEmail);

            await client.SendMailAsync(msg);
            return true;
        }
    }
}
