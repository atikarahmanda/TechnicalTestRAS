using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using FinalProjectRAS.ViewModels;

namespace FinalProjectRAS.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, DataEmailVM data)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            var builder = new BodyBuilder
            {
                HtmlBody = EmailTemplate.BuildEmailBody(data)

            };

            // Embed the logo image as a linked resource
            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Services", "logo.png");
            var logo = builder.LinkedResources.Add(logoPath);
            logo.ContentId = "logo";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
