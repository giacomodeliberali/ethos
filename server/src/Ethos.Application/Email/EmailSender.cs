using System.Threading.Tasks;
using Ethos.Common;
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

        public async Task SendEmail(string recipient, string subject, string message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailConfig.Name, _emailConfig.UserName));
            mimeMessage.To.Add(new MailboxAddress(string.Empty, recipient));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message,
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpServerPort, true);

            await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }
    }
}
