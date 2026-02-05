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

        public async Task<bool> SendEmail(string fromEmail, string toEmail, string subject,
            string cCEmail, string body, CancellationToken cancellationToken)
        {
            var smtpServer = _configuration["SmtpSettings:Server"] ?? "localhost";
            var smtpPortString = _configuration["SmtpSettings:Port"] ?? "25";
            var smtpPort = int.TryParse(smtpPortString, out var port) ? port : 25;
            var smtpUsername = _configuration["SmtpSettings:Username"] ?? "";
            var smtpPassword = _configuration["SmtpSettings:Password"] ?? "";
            var bccEmail = _configuration["FlightAdmin:BccEmail"] ?? "";

            Console.WriteLine($"✅ Sending Mail To = {toEmail}");
            Console.WriteLine($"✅ Sending Mail From = {fromEmail}");
            Console.WriteLine($"✅ CC = {cCEmail}");
            Console.WriteLine($"✅ BCC = {bccEmail}");

            //created object of SmtpClient details and provides server details
            SmtpClient MyServer = new SmtpClient();
            MyServer.Host = smtpServer;
            MyServer.Port = smtpPort;
            MyServer.EnableSsl = true;

            //Server Credentials
            NetworkCredential NC = new NetworkCredential();
            NC.UserName = smtpUsername;
            NC.Password = smtpPassword;
            //assigned credetial details to server
            MyServer.Credentials = NC;


            //create sender address
            MailAddress from = new MailAddress(fromEmail, fromEmail);

            //create receiver address
            MailAddress to = new MailAddress(toEmail, toEmail);

            MailMessage Mymessage = new MailMessage(from, to);
            Mymessage.Subject = subject.Trim();
            Mymessage.Body = body;
            Mymessage.IsBodyHtml = true;
            Mymessage.BodyEncoding = System.Text.Encoding.UTF8;
            Mymessage.SubjectEncoding = System.Text.Encoding.UTF8;

            if (!string.IsNullOrEmpty(cCEmail))
            {
                Mymessage.CC.Add(cCEmail);
            }
            if (!string.IsNullOrEmpty(bccEmail))
            {
                Mymessage.Bcc.Add(bccEmail);
            }
            //sends the email
            await MyServer.SendMailAsync(Mymessage).ConfigureAwait(false);
            return true;
        }
    }
}

