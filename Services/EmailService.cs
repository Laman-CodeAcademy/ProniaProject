using MailKit.Net.Smtp;
using MimeKit;
using Pronia.Abstraction;
using Pronia.ViewModels.EmailViewModels;
using System.Net.Mail;

namespace Pronia.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpSettingsVM _settings;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _settings = _configuration.GetSection("SmtpSettings").Get<SmtpSettingsVM>() ?? new();
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(email, email));

            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };


            using var client = new MailKit.Net.Smtp.SmtpClient();

            client.ServerCertificateValidationCallback = (x, y, z, i) => true;

            await client.ConnectAsync(_settings.Server, _settings.Port, true);


            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);

        }

    }
}
