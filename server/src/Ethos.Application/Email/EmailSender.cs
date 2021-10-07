using System.Threading.Tasks;
using Ethos.Shared;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Ethos.Application.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig _emailConfig;

        public EmailSender(IOptions<EmailConfig> emailConfigOptions)
        {
            _emailConfig = emailConfigOptions.Value;
        }

        public async Task SendEmail(string recipient, string subject, string text)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfig.Name, _emailConfig.UserName));
            message.To.Add(new MailboxAddress(string.Empty, recipient));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = text,
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpServerPort, true);

            await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
